using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebUIApp.BizLogic;
using WebUIApp.Models;

namespace WebUIApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ServiceAClient serviceAclient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ServiceAClient serviceAclient)
        {
            this.serviceAclient = serviceAclient;
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
            var color = await serviceAclient.GetColor();
            return PartialView(color);
        }
    }
}
