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
    public class CandlesController : Controller
    {
        private readonly diplomContext _context;
        private readonly InvestApiClient _investApi;
        private readonly IConfiguration _configuration;

        public CandlesController(diplomContext context, InvestApiClient investApi, IConfiguration configuration)
        {
            _context = context;
            _investApi = investApi;
            _configuration = configuration;
        }

        // GET: Candles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Candles.ToListAsync());
        }

        // GET: Candles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candle = await _context.Candles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candle == null)
            {
                return NotFound();
            }

            return View(candle);
        }

        // GET: Candles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Candles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Open,Close,High,Low,Time,Volume")] Candle candle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(candle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(candle);
        }

        // GET: Candles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candle = await _context.Candles.FindAsync(id);
            if (candle == null)
            {
                return NotFound();
            }
            return View(candle);
        }

        // POST: Candles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Open,Close,High,Low,Time,Volume")] Candle candle)
        {
            if (id != candle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(candle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandleExists((int)candle.Id))
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
            return View(candle);
        }

        // GET: Candles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candle = await _context.Candles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candle == null)
            {
                return NotFound();
            }

            return View(candle);
        }

        // POST: Candles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candle = await _context.Candles.FindAsync(id);
            _context.Candles.Remove(candle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandleExists(int id)
        {
            return _context.Candles.Any(e => e.Id == id);
        }
        
        public async Task UpdateCandles(Share share)
        {
            GetCandlesRequest candlesRequest = new GetCandlesRequest();
            candlesRequest.Figi = share.Figi;
            candlesRequest.Interval = CandleInterval.Hour;
            int startYear = int.Parse(_configuration["ParsingPeriod:Start:year"]);
            int startMonth = int.Parse(_configuration["ParsingPeriod:Start:month"]);
            int startDay = int.Parse(_configuration["ParsingPeriod:Start:day"]);
            DateTime startParsingDate = new DateTime(startYear, startMonth, startDay);
            if (share.Candles.Count() > 0)
                startParsingDate = share.Candles.Last().Time;
            while (startParsingDate < DateTime.Now)
            {
                DateTime tillParsingDate = startParsingDate.AddDays(1);
                candlesRequest.From = Timestamp.FromDateTime(startParsingDate.ToUniversalTime());
                candlesRequest.To = Timestamp.FromDateTime(tillParsingDate.ToUniversalTime());
                GetCandlesResponse candlesResponse = _investApi.MarketData.GetCandles(candlesRequest);
                foreach (ApiCandle apiCandle in candlesResponse.Candles)
                {
                    Candle candle = new Candle(apiCandle);
                    if (candle.Time <= startParsingDate)
                        break;
                    share.Candles.Add(candle);
                    _context.Candles.Add(candle);
                }
                startParsingDate = tillParsingDate;
                await _context.SaveChangesAsync();
                Thread.Sleep(500);
            }

            await _context.SaveChangesAsync();
        }
    }
}
