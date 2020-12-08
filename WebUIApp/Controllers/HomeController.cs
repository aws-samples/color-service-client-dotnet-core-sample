using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebUIColorClient.Models;
using WebUIColorClient.BizLogic;

namespace WebUIColorClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ColorServiceClient _colorServiceClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ColorServiceClient colorServiceClient)
        {
            this._colorServiceClient = colorServiceClient;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

        public async Task<ActionResult> Color()
        {
            var color = await _colorServiceClient.GetColor();
            return PartialView(color);
        }
    }
}
