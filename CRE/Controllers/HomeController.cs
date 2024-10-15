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
                // Store user information in ViewBag for use in the view
                ViewBag.User = new
                {
                    fName = user.fName,
                    mName = user.mName,
                    lName = user.lName,
                    type = user.type
                };

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRoles = roles; // Store roles in ViewBag for use in the view
            }
            else
            {
                ViewBag.UserRoles = new List<string>(); // No user logged in
                ViewBag.User = null; // Clear user data if not logged in
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
