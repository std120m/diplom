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

namespace diplom.Controllers
{
    public class CompanyEventsController : Controller
    {
        private readonly diplomContext _context;

        public CompanyEventsController(diplomContext context)
        {
            _context = context;
        }

        // GET: CompanyEvents
        public async Task<IActionResult> Index()
        {
            return View(await _context.CompanyEvents.ToListAsync());
        }

        // GET: CompanyEvents/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyEvents = await _context.CompanyEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyEvents == null)
            {
                return NotFound();
            }

            return View(companyEvents);
        }

        // GET: CompanyEvents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CompanyEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Name")] CompanyEvents companyEvents)
        {
            if (ModelState.IsValid)
            {
                _context.Add(companyEvents);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(companyEvents);
        }

        // GET: CompanyEvents/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyEvents = await _context.CompanyEvents.FindAsync(id);
            if (companyEvents == null)
            {
                return NotFound();
            }
            return View(companyEvents);
        }

        // POST: CompanyEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Date,Name")] CompanyEvents companyEvents)
        {
            if (id != companyEvents.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyEvents);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyEventsExists(companyEvents.Id))
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
            return View(companyEvents);
        }

        // GET: CompanyEvents/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyEvents = await _context.CompanyEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyEvents == null)
            {
                return NotFound();
            }

            return View(companyEvents);
        }

        // POST: CompanyEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var companyEvents = await _context.CompanyEvents.FindAsync(id);
            _context.CompanyEvents.Remove(companyEvents);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyEventsExists(long id)
        {
            return _context.CompanyEvents.Any(e => e.Id == id);
        }
    }
}
