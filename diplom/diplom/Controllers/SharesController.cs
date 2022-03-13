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
using Share = diplom.Models.Share;

namespace diplom.Controllers
{
    public class SharesController : Controller
    {
        private readonly diplomContext _context;
        private readonly InvestApiClient _investApi;

        public SharesController(diplomContext context, InvestApiClient investApi)
        {
            _context = context;
            _investApi = investApi;
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

                    IQueryable<Share> shares = from s in _context.Shares select s;
                    Share share = null;
                    if (shares.Count() > 0 && shares.First(shares => shares.Figi == apiShare.Figi) != null)
                    {
                        share = shares.First(shares => shares.Figi == apiShare.Figi);
                        share.Update(apiShare);
                        share.Exchange = exchange;
                        _context.Shares.Update(share);
                    }
                    else
                    {
                        share = new Share(apiShare);
                        share.Exchange = exchange;
                        _context.Shares.Add(share);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
