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
            try
            {
                ViewBag.Shares = new SharesController(_context).GetShares();
                ViewData["candles"] = new CandlesController(_context).GetCandles();
                ViewData["counties"] = new CountriesController(_context).GetCounties();
                ViewData["exchanges"] = new ExchangesController(_context).GetExchanges();


                return View();
            } catch (Exception e)
            {
                return View("~/Views/Errors/500.cshtml");
            }
        }

        //[HttpGet("share/{id?}")]
        public IActionResult Share(int? id = null)
        {
            Models.Share? share = _context.Shares.Find(3);
            if (share == null)
                return NotFound();
            ViewData["share"] = share;
            return View(share);
        }

        public Microsoft.AspNetCore.Mvc.JsonResult GetCandles()
        {
            this.ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            this.ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
            this.ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");

            Microsoft.Extensions.Primitives.StringValues currentChartType;
            if (!Request.Query.TryGetValue("currentChartType", out currentChartType))
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
            //List<WorldNews> news = _context.WorldNews.ToList();
            List<WorldNews> news = null;
            List<object[]> candles = new List<object[]>();
            foreach (Models.Share share in shares)
            {
                object[] shareInfo = new object[2];
                shareInfo[0] = share.Name ?? "";
                if (currentChartType[0] == "trend")
                    shareInfo[1] = share.GetCandlesByDay(_context);
                else
                    shareInfo[1] = share.GetCandlesArray();
                candles.Add(shareInfo);
            }
            object[] result = new object[2];
            result[0] = news;
            result[1] = candles;
            return Json(result, options);
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