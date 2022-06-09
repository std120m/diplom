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
using System.Text.RegularExpressions;
using System.Text;
using diplom.Helpers;
using System.Net;

namespace diplom.Controllers
{
    public class WorldNewsController : Controller
    {
        private readonly diplomContext _context;
        private readonly IConfiguration _configuration;

        public WorldNewsController(diplomContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: WorldNews
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorldNews.ToListAsync());
        }

        // GET: WorldNews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldNews = await _context.WorldNews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worldNews == null)
            {
                return NotFound();
            }

            return View(worldNews);
        }

        // GET: WorldNews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorldNews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime")] WorldNews worldNews)
        {
            if (ModelState.IsValid)
            {
                _context.Add(worldNews);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(worldNews);
        }

        // GET: WorldNews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldNews = await _context.WorldNews.FindAsync(id);
            if (worldNews == null)
            {
                return NotFound();
            }
            return View(worldNews);
        }

        // POST: WorldNews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime")] WorldNews worldNews)
        {
            if (id != worldNews.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worldNews);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorldNewsExists(worldNews.Id))
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
            return View(worldNews);
        }

        // GET: WorldNews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldNews = await _context.WorldNews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worldNews == null)
            {
                return NotFound();
            }

            return View(worldNews);
        }

        // POST: WorldNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worldNews = await _context.WorldNews.FindAsync(id);
            _context.WorldNews.Remove(worldNews);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorldNewsExists(int id)
        {
            return _context.WorldNews.Any(e => e.Id == id);
        }

        public async Task UpdateWorldNews()
        {
            IQueryable<WorldNews> worldNews = _context.WorldNews;
            WorldNews lastWorldNews = worldNews.Any() ? worldNews.OrderBy(news => news.DateTime).Last() : null;

            int startYear = int.Parse(_configuration["ParsingPeriod:Start:year"]);
            int startMonth = int.Parse(_configuration["ParsingPeriod:Start:month"]);
            int startDay = int.Parse(_configuration["ParsingPeriod:Start:day"]);
            DateTime parsingDate = new DateTime(startYear, startMonth, startDay);

            if (lastWorldNews != null)
                parsingDate = lastWorldNews.DateTime;

            while (parsingDate != DateTime.Now)
            {
                string formatDate = parsingDate.ToString("dd");
                string formatMonth = parsingDate.ToString("MM");
                string url = $"/news/{startYear}/{formatMonth}/{formatDate}";

                ParseWorldNews(url, parsingDate);
                parsingDate = parsingDate.AddDays(1);
                await _context.SaveChangesAsync();
            }
        }

        private void ParseWorldNews(string url, DateTime parsingDate)
        {
            string pattern = "item news.*?class=\"time\">(.*?)<.*?<a class=\"titles\" href=\"(.*?)\">.*?class=\"card-title\">(.*?)<";
            string formatDate = parsingDate.ToString("dd");
            string formatMonth = parsingDate.ToString("MM");
            string text = Helper.GetStringFromHtml(_configuration["WorldNewsDomain"] + url, Encoding.GetEncoding(65001));
            foreach (Match match in Regex.Matches(text, pattern))
            {
                try
                {
                    string title = match.Groups[3].Value;
                    string[] time = match.Groups[1].Value.Split(':');
                    DateTime newsPublicationDate = parsingDate.AddHours(int.Parse(time[0]));
                    newsPublicationDate = newsPublicationDate.AddMinutes(int.Parse(time[1]));
                    string newsUrl = _configuration["WorldNewsDomain"] + match.Groups[2].Value;
                    Thread.Sleep(400);
                    string newsText = Helper.GetStringFromHtml(newsUrl, Encoding.GetEncoding(65001));
                    string newsTextPattern = "\"articleBody\":\"(.*?)\"}";
                    foreach (Match textMatch in Regex.Matches(newsText, newsTextPattern))
                    {
                        newsText = textMatch.Groups[1].Value;
                    }

                    WorldNews news = new WorldNews(newsPublicationDate, newsUrl, newsText, title);
                    _context.WorldNews.Add(news);
                }
                catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
            }

            string moreNewsPattern = "<a class=\"button-more-news__link\" data-path=\"(.*?)\"";
            foreach (Match match in Regex.Matches(text, moreNewsPattern))
            {
                try
                {
                    string moreNewsUrl = match.Groups[1].Value;
                    ParseWorldNews(moreNewsUrl, parsingDate);
                }
                catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
            }
        }
    }
}
