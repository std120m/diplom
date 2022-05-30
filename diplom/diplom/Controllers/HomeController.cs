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

namespace diplom.Controllers
{
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
            Models.Share? share = _context.Shares.Find(3);
            if (share == null)
                return NotFound();
            ViewData["share"] = share.Name;
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            ViewData["candles"] = Json(share.GetCandlesByDay(_context), options).Value;
            return View();
        }

        public Microsoft.AspNetCore.Mvc.JsonResult GetCandles()
        {
            Models.Share? share = _context.Shares.Find(5);
            if (share == null)
                return Json("Share not found");
            //ViewData["share"] = share.Name;
            //ViewData["candles"] = Json(share.Candles, JsonRequestBehavior.AllowGet);
            //share = new Models.Share { Figi = "werwerw", Name = "sfsdf" };
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return Json(share.GetCandlesByDay(_context), options);
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