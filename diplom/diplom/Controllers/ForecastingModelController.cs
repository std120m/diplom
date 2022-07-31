using diplom.Data;
using diplom.Models;
using Microsoft.AspNetCore.Mvc;

namespace diplom.Controllers
{
    public class ForecastingModelController : Controller
    {
        private readonly diplomContext _context;
        private readonly IConfiguration _configuration;

        public ForecastingModelController(diplomContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
