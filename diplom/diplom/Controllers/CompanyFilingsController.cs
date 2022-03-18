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
    public class CompanyFilingsController : Controller
    {
        private readonly diplomContext _context;

        public CompanyFilingsController(diplomContext context)
        {
            _context = context;
        }

        // GET: CompanyFilings
        public async Task<IActionResult> Index()
        {
            return View(await _context.CompanyFilings.ToListAsync());
        }

        // GET: CompanyFilings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyFilings = await _context.CompanyFilings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyFilings == null)
            {
                return NotFound();
            }

            return View(companyFilings);
        }

        // GET: CompanyFilings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CompanyFilings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Type,Title,Url")] CompanyFilings companyFilings)
        {
            if (ModelState.IsValid)
            {
                _context.Add(companyFilings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(companyFilings);
        }

        // GET: CompanyFilings/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyFilings = await _context.CompanyFilings.FindAsync(id);
            if (companyFilings == null)
            {
                return NotFound();
            }
            return View(companyFilings);
        }

        // POST: CompanyFilings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Date,Type,Title,Url")] CompanyFilings companyFilings)
        {
            if (id != companyFilings.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyFilings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyFilingsExists(companyFilings.Id))
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
            return View(companyFilings);
        }

        // GET: CompanyFilings/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyFilings = await _context.CompanyFilings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyFilings == null)
            {
                return NotFound();
            }

            return View(companyFilings);
        }

        // POST: CompanyFilings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var companyFilings = await _context.CompanyFilings.FindAsync(id);
            _context.CompanyFilings.Remove(companyFilings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyFilingsExists(long id)
        {
            return _context.CompanyFilings.Any(e => e.Id == id);
        }
    }
}
