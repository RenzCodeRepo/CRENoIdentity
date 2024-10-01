using Microsoft.Extensions.Configuration; // Import this namespace
using CRE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CRE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration; // Declare a variable to hold the configuration

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // Get the development user ID from the config file
            var devUserId = _configuration["DevelopmentUserId"];

            ViewBag.DevUserId = devUserId; // Just to display the ID in the view

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
    }
}
