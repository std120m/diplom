using diplom.Data;
using diplom.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web.Mvc;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Candle = diplom.Models.Candle;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace diplom.Controllers
{
    //[Route("/")]
    public class HomeController : Controller
    {
        private readonly diplomContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly InvestApiClient _investApi;

        public HomeController(diplomContext context, ILogger<HomeController> logger, InvestApiClient investApi)
        {
            _context = context;
            _logger = logger;
            _investApi = investApi;
        }

        public IActionResult Index()
        {
            List<Models.Share> shares = _context.Shares.ToList();
            //if (share == null)
            //    return NotFound();
            //ViewData["share"] = share.Name;
            //JsonSerializerOptions options = new()
            //{
            //    ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //    WriteIndented = true
            //};
            ViewData["shares"] = shares;
            return View();
        }

        //[HttpGet("share/{id?}")]
        public IActionResult Share(int? id = null)
        {
            Models.Share? share = _context.Shares.Find(3);
            if (share == null)
                return NotFound();
            //ViewData["share"] = share.Name;
            //JsonSerializerOptions options = new()
            //{
            //    ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //    WriteIndented = true
            //};
            ViewData["share"] = share;
            return View(share);
        }

        public Microsoft.AspNetCore.Mvc.JsonResult GetCandles()
        {
            Microsoft.Extensions.Primitives.StringValues period;
            if (!Request.Query.TryGetValue("period", out period))
                return Json(null);
            string[] shareIds = Request.Query.ToList()[1].Value.ToArray();

            List<Models.Share> shares = _context.Shares.Where(s => shareIds.Contains(s.Id.ToString())).ToList();
            if (shares.Count == 0)
                return Json("Shares not found");
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            List<WorldNews> news = _context.WorldNews.ToList();
            List<object[]> candles = new List<object[]>();
            foreach (Models.Share share in shares)
            {
                object[] shareInfo = new object[2];
                shareInfo[0] = share.Name ?? "";
                shareInfo[1] = share.GetCandlesByDay(_context);
                candles.Add(shareInfo);
            }
            object[] result = new object[2];
            result[0] = news;
            result[1] = candles;
            return Json(result, options);
            //return Json(share.GetCandlesByDay(_context), options);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}