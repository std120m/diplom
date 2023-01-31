#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using diplom.Data;
using diplom.Models;
using Tinkoff.InvestApi.V1;
using Tinkoff.InvestApi;
using ApiShare = Tinkoff.InvestApi.V1.Share;
using ApiCandle = Tinkoff.InvestApi.V1.HistoricCandle;
using Share = diplom.Models.Share;
using Candle = diplom.Models.Candle;
using Google.Protobuf.WellKnownTypes;

namespace diplom.Controllers
{
    public class SharesController : Controller
    {
        private readonly diplomContext _context;
        private readonly InvestApiClient _investApi;
        private readonly IConfiguration _configuration;

        public SharesController(diplomContext context)
        {
            _context = context;
        }

        //public SharesController(diplomContext context, InvestApiClient investApi, IConfiguration configuration)
        //{
        //    _context = context;
        //    _investApi = investApi;
        //    _configuration = configuration;
        //}

        // GET: Shares
        public async Task<IActionResult> Index()
        {
            ViewBag.Sectors = new SectorsController(_context).GetSectors();
            return View(await _context.Shares.ToListAsync());
        }

        // GET: Shares/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var share = await _context.Shares
                .FirstOrDefaultAsync(m => m.Id == id);
            if (share == null)
            {
                return NotFound();
            }

            foreach (var news in _context.WorldNews.ToList())
            //foreach (var news in _context.WorldNews.Where(news => news.DateTime.Date == DateTime.Now.Date).ToList())
            {
                new WorldNewsController(_context, _configuration).GetWorldnewsImpact(news.Id, id);
            }

            var weekForecast = new ForecastingModel(ForecastingModel.ForecastHorizon.week).GetForecast(share.Id);
            var monthForecast = new ForecastingModel(ForecastingModel.ForecastHorizon.month).GetForecast(share.Id);
            var halfYearForecast = new ForecastingModel(ForecastingModel.ForecastHorizon.halfYear).GetForecast(share.Id);

            ViewBag.WeekForecastDelta = GetForecastDelta(weekForecast);
            ViewBag.MonthForecastDelta = GetForecastDelta(monthForecast);
            ViewBag.HalfYearForecastDelta = GetForecastDelta(halfYearForecast);

            ViewBag.WeekForecast = weekForecast.ForecastedClose.ToList().Last();
            ViewBag.MonthForecast = monthForecast.ForecastedClose.ToList().Last();
            ViewBag.HalfYearForecast = halfYearForecast.ForecastedClose.ToList().Last();
            return View(share);
        }

        // GET: Shares/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shares/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Figi,Ticker,ClassCode,Currency,Name,IpoDate,IssueSize,IssuePlanSize,ShareType")] Share share)
        {
            if (ModelState.IsValid)
            {
                _context.Add(share);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(share);
        }

        // GET: Shares/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var share = await _context.Shares.FindAsync(id);
            if (share == null)
            {
                return NotFound();
            }
            return View(share);
        }

        // POST: Shares/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Figi,Ticker,ClassCode,Currency,Name,IpoDate,IssueSize,IssuePlanSize,ShareType")] Share share)
        {
            if (id != share.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(share);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShareExists(share.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(share);
        }

        // GET: Shares/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var share = await _context.Shares
                .FirstOrDefaultAsync(m => m.Id == id);
            if (share == null)
            {
                return NotFound();
            }

            return View(share);
        }

        // POST: Shares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var share = await _context.Shares.FindAsync(id);
            _context.Shares.Remove(share);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShareExists(int id)
        {
            return _context.Shares.Any(e => e.Id == id);
        }

        public List<Share> GetShares() 
        {
            return _context.Shares.ToList();
        }

        private double GetForecastDelta(ModelOutput forecast)
        {
            var weekForecastDelta = Math.Round(((double)forecast.ForecastedClose.ToList().Last()- (double)forecast.LowerBoundClose.ToList().Last()) / (double)forecast.LowerBoundClose.ToList().Last() * 100, 2);
            weekForecastDelta += Math.Round(((double)forecast.ForecastedClose.ToList().Last() - (double)forecast.UpperBoundClose.ToList().Last()) / (double)forecast.UpperBoundClose.ToList().Last() * 100, 2);
            weekForecastDelta /= 2;

            return weekForecastDelta;
        }
    }
}
