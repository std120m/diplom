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
using diplom.Models.SentimentPrediction;
using Pullenti.Ner.Org;
using Pullenti.Ner.Geo;
using DeepMorphy;
using DeepMorphy.Model;

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

            var newsQuotesImpacts = from impact in worldNews.NewsQuotesImpacts.DistinctBy(_impact => _impact.CompanyId)
                                    orderby Math.Abs(impact.Influence) descending
                                    orderby impact.Influence descending
                                    select impact;
            ViewBag.ImpactCompanies = newsQuotesImpacts.Take(10).ToArray();

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

        public List<WorldNews> GetWorldNews(DateTime? date = null)
        {
            var query =  _context.WorldNews;
            if (date != null)
                return query.Where(news => news.DateTime.Date == date.Value.Date).ToList();
            else
                return query.ToList();
        }

        private bool WorldNewsExists(int id)
        {
            return _context.WorldNews.Any(e => e.Id == id);
        }

        public async Task UpdateWorldNews()
        {
            IQueryable<WorldNews> worldNews = _context.WorldNews;
            WorldNews lastWorldNews = worldNews.Any() ? worldNews.OrderBy(news => news.DateTime).Last() : null;

            DateTime parsingDate = new DateTime();
            if (lastWorldNews == null)
            {
                int startYear = int.Parse(_configuration["ParsingPeriod:Start:year"]);
                int startMonth = int.Parse(_configuration["ParsingPeriod:Start:month"]);
                int startDay = int.Parse(_configuration["ParsingPeriod:Start:day"]);
                parsingDate = new DateTime(startYear, startMonth, startDay);
            }
            else
            {
                parsingDate = lastWorldNews.DateTime;
            }
            parsingDate = parsingDate.AddDays(1);

            while (parsingDate.Date <= DateTime.Now.Date)
            {
                string formatDate = parsingDate.ToString(@"yyyy\/MM\/dd");
                string url = $"/news/{formatDate}";

                ParseWorldNews(url, parsingDate);
                parsingDate = parsingDate.AddDays(1);
                await _context.SaveChangesAsync();
            }
        }

        private void ParseWorldNews(string url, DateTime parsingDate)
        {
            string pattern = "item news.*?class=\"time\">(.*?)<.*?<a class=\"titles\" href=\"(.*?)\">.*?class=\"card-title\">(.*?)<";
            pattern = "_news.*?card-full-news__date\">(.*?)<.*?<a .*? href=\"(.*?)\">.*?card-full-news__title\">(.*?)<";
            string formatDate = parsingDate.ToString("dd");
            string formatMonth = parsingDate.ToString("MM");
            string html = Helper.GetStringFromHtml(_configuration["WorldNewsDomain"] + url, Encoding.GetEncoding(65001));
            foreach (Match match in Regex.Matches(html, pattern))
            {
                try
                {
                    bool isEconomyNews = true;
                    string title = match.Groups[3].Value;
                    string[] time = match.Groups[1].Value.Split(',')[0].Split(':');
                    DateTime newsPublicationDate = parsingDate.AddHours(int.Parse(time[0]));
                    newsPublicationDate = newsPublicationDate.AddMinutes(int.Parse(time[1]));
                    string newsUrl = _configuration["WorldNewsDomain"] + match.Groups[2].Value;
                    Thread.Sleep(400);
                    string? newsText = Helper.GetStringFromHtml(newsUrl, Encoding.GetEncoding(65001));
                    if (newsText == null)
                        continue;
                    string newsRubricPattern = "<a class=\"topic-header__item topic-header__rubric\".*?>(.*?)<";
                    foreach (Match rubric in Regex.Matches(newsText, newsRubricPattern))
                    {
                        if (rubric.Groups[1].Value != "Экономика")
                            isEconomyNews = false;
                    }
                    if (! isEconomyNews)
                        continue;
                    string newsTextPattern = "\"articleBody\":\"(.*?)\"";
                    foreach (Match textMatch in Regex.Matches(newsText, newsTextPattern))
                    {
                        newsText = textMatch.Groups[1].Value;
                    }

                    if (_context.WorldNews.Where(news => news.Url == newsUrl).Count() == 0)
                    {
                        WorldNews news = new WorldNews(newsPublicationDate, newsUrl, newsText, title);
                        _context.WorldNews.Add(news);
                    }
                }
                catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
            }
        }

        public void GetWorldnewsImpact(int newsId, int? shareId = null)
        {
            WorldNews worldNews = _context.WorldNews.Where(news => news.Id == newsId).ToList().First();
            if (worldNews == null)
                return;
            List<WorldNews> allNews = new List<WorldNews>() { worldNews };//_context.WorldNews.Where(news => news.Id > 29985).ToList();
            List<EntitySentimentPrediction> predictions = new List<EntitySentimentPrediction>();
            foreach (var news in allNews)
            {
                if (news.Text.Contains("articleBody"))
                {
                    string newsTextPattern = "\"articleBody\":\"(.*?)\"";
                    foreach (Match textMatch in Regex.Matches(news.Text, newsTextPattern))
                    {
                        news.Text = textMatch.Groups[1].Value;
                        _context.WorldNews.Update(news);
                    }
                }

                SentimentPredictionModel model = new SentimentPredictionModel();
                predictions = model.Predict(news.Text);
                if (predictions.Count > 0)
                {
                    MorphAnalyzer morph;
                    List<MorphInfo> results = new List<MorphInfo>();
                    try
                    {
                        morph = new MorphAnalyzer(withLemmatization: true);
                        results = morph.Parse(news.Text.Split(' ')).ToList();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    var keywords = new List<string>();
                    foreach (var morphInfo in results)
                    {
                        if (keywords.Count >= 5)
                            continue;
                        var tag = morphInfo.BestTag;
                        if (tag.HasLemma && tag.Power > 0.98)
                        {
                            Console.WriteLine($"{morphInfo.Text}:");
                            Console.WriteLine($"    {tag} : {tag.Power}");
                            keywords.Add(tag.Lemma);
                        }
                    }
                    List<NewsQuotesImpact> impacts = new List<NewsQuotesImpact>();
                    foreach (var prediction in predictions)
                    {
                        //bool isGeo = false;
                        List<Share> shares = shareId != null ? _context.Shares.Where(share => share.Id == shareId).ToList() : _context.Shares.ToList();
                        //List<Country> countries = new List<Country>();
                        //var entity = prediction.Entity.Entity;
                        List<Candle> candles = new List<Candle>();// _context.Candles.Where(candle => candle.Time > news.DateTime.AddDays(-1).Date && candle.Time < news.DateTime.AddDays(5).Date).ToList();
                        foreach (var share in shares)
                        {
                            if (share.Company == null || share.Company.BrandInfo == null)
                                continue;

                            var companyMorphInfo = morph.Parse(share.Company.BrandInfo.Replace("  ", " ").Split(' ')).ToList();
                            var companyKeywords = new List<string>();
                            foreach (var morphInfo in companyMorphInfo)
                            {
                                if (companyKeywords.Count >= 5)
                                    continue;
                                var tag = morphInfo.BestTag;
                                if (tag.HasLemma && tag.Power > 0.5 && tag.Power < 0.7)
                                {
                                    Console.WriteLine($"{morphInfo.Text}:");
                                    Console.WriteLine($"    {tag} : {tag.Power}");
                                    companyKeywords.Add(tag.Lemma);
                                }
                            }

                            candles = share.Candles.Where(candle => candle.Time > news.DateTime.AddDays(1).Date).ToList();
                            if (candles.Count == 0)
                            {
                                SaveImpactForLastNews(share, news, companyKeywords, impacts);
                            }
                            else
                            {
                                candles = share.Candles.Where(candle => candle.Time > news.DateTime.AddDays(-1).Date && candle.Time < news.DateTime.AddDays(5).Date).ToList();
                                if (candles.Count > 0)
                                {
                                    Candle firstCandle = candles.First();
                                    Candle lastCandle = candles.Last();
                                    double? influence = ((firstCandle.Close - lastCandle.Close) * 100) / (firstCandle.Close);
                                    if (influence != null && Math.Abs((double)influence) > 5)
                                    {
                                        impacts.Add(new NewsQuotesImpact(share.Company, news, (double)influence));
                                    }
                                }
                            }
                        }
                    }
                    if (impacts.Count > 0)
                    {
                        foreach (var keyword in keywords)
                        {
                            Keyword dbKeyword = null;
                            if (_context.Keywords.Where(k => k.Value == keyword).Count() > 0)
                            {
                                dbKeyword = _context.Keywords.Where(k => k.Value == keyword).First();
                            }
                            else
                            {
                                dbKeyword = new Keyword(keyword);
                                _context.Keywords.Add(dbKeyword);
                                _context.SaveChanges();
                            }
                            _context.WorldNewsKeywords.Add(new WorldNewsKeyword(news, dbKeyword));
                            _context.SaveChanges();
                        }
                        _context.NewsQuotesImpacts.AddRange(impacts);
                        _context.SaveChanges();
                    }
                }
            }
        }

        private void SaveImpactForLastNews(Share share, WorldNews news, List<string> companyKeywords, List<NewsQuotesImpact> impacts)
        {
            double newsImpact = 0;
            foreach (var impact in share.Company.NewsQuotesImpacts)
            {
                double keywordsImpact = 0;
                foreach (var keyword in companyKeywords)
                {
                    double oneKeywordImpact = 0;
                    var keywordsCount = impact.WorldNews.Keywords.Where(kw => kw.Keyword.Value == keyword).ToList().Count();
                    if (keywordsCount > 0)
                    {
                        oneKeywordImpact = impact.Influence * Math.Abs(impact.Influence / keywordsCount);
                    }
                    keywordsImpact += oneKeywordImpact;
                }
                newsImpact += keywordsImpact / companyKeywords.Count();
            }
            if (newsImpact != 0)
            {
                impacts.Add(new NewsQuotesImpact(share.Company, news, newsImpact));
            }
        }
    }
}
