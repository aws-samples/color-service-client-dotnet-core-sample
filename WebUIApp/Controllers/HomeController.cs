using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebUIApp.Models;
using WebUIApp.Utility;

namespace WebUIApp.Controllers
{
    public class HomeController : Controller
    {
        public IOptionsSnapshot<Settings> Settings { get; }
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IOptionsSnapshot<WebUIApp.Models.Settings> settings)
        {
            Settings = settings;
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
            string colorString = "DarkRed";
            try
            {
                //using (var svcClient = new ServiceA(new Uri(this.Settings.Value.ServiceBaseUrl)))
                //{
                //    var colorResource = new Color(svcClient);
                //    colorString = await colorResource.GetAsync();
                //}
            }
            catch (Exception e)
            {
                colorString = e.Innermost().Message;
            }

            var color = new Models.Color { Value = colorString };

            return PartialView(color);
        }
    }
}
