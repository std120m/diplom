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

        public SharesController(diplomContext context, InvestApiClient investApi, IConfiguration configuration)
        {
            _context = context;
            _investApi = investApi;
            _configuration = configuration;
        }

        // GET: Shares
        public async Task<IActionResult> Index()
        {
            await UpdateShares();
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

        public async Task<IActionResult> UpdateShares(int? id = null)
        {
            if (id == null)
            {
                SharesResponse sharesResponse = _investApi.Instruments.Shares();
                foreach (ApiShare apiShare in sharesResponse.Instruments)
                {
                    IQueryable<Exchange> exchanges = from e in _context.Exchanges select e;
                    Exchange exchange = null;
                    if (exchanges.Count() > 0 && exchanges.First(exchange => exchange.Name == apiShare.Exchange) != null)
                    {
                        exchange = exchanges.First(exchange => exchange.Name == apiShare.Exchange);
                    }
                    else
                    {
                        exchange = new Exchange(apiShare.Exchange);
                        _context.Exchanges.Add(exchange);
                    }

                    IQueryable<Country> countries = from c in _context.Countries select c;
                    Country country = null;
                    if (countries.Count() > 0 && countries.First(country => country.Name == apiShare.CountryOfRiskName && country.Code == apiShare.CountryOfRisk) != null)
                    {
                        country = countries.First(country => country.Name == apiShare.CountryOfRiskName && country.Code == apiShare.CountryOfRisk);
                    }
                    else
                    {
                        country = new Country(apiShare.CountryOfRiskName, apiShare.CountryOfRisk);
                        _context.Countries.Add(country);
                    }

                    IQueryable<Sector> sectors = from s in _context.Sectors select s;
                    Sector sector = null;
                    if (sectors.Count() > 0 && sectors.First(sector => sector.Name == apiShare.Sector) != null)
                    {
                        sector = sectors.First(sector => sector.Name == apiShare.Sector);
                    }
                    else
                    {
                        sector = new Sector(apiShare.Sector);
                        _context.Sectors.Add(sector);
                    }

                    IQueryable<Share> shares = from s in _context.Shares select s;
                    Share share = null;
                    if (shares.Count() > 0 && shares.First(shares => shares.Figi == apiShare.Figi) != null)
                    {
                        share = shares.First(shares => shares.Figi == apiShare.Figi);
                        share.Update(apiShare);
                        share.Exchange = exchange;
                        share.Country = country;
                        share.Sector = sector;
                        share.Candles = GetCandles(share);
                        _context.Shares.Update(share);
                    }
                    else
                    {
                        share = new Share(apiShare);
                        share.Exchange = exchange;
                        share.Country = country;
                        share.Sector = sector;
                        share.Candles = GetCandles(share);
                        _context.Shares.Add(share);
                    }                    
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private List<Candle> GetCandles(Share share)
        {
            List<Candle> candles = new List<Candle>();

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
                startParsingDate = tillParsingDate;
                foreach (ApiCandle apiCandle in candlesResponse.Candles)
                {
                    Candle candle = new Candle(apiCandle);
                    candles.Add(candle);
                    _context.Candles.Add(candle);
                }
            }

            return candles;
        }
    }
}
