using Microsoft.AspNetCore.Identity; // Import the Identity namespace
using Microsoft.Extensions.Configuration;
using CRE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CRE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration; // Configuration variable
        private readonly UserManager<AppUser> _userManager; // Add UserManager for identity user

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager; // Initialize UserManager
        }

        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // You can access user information or roles here
                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRoles = roles; // Store roles in ViewBag for use in the view
                ViewBag.UserId = user.Id; // Store user ID in ViewBag
            }
            else
            {
                ViewBag.UserRoles = new List<string>(); // No user logged in
            }

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
