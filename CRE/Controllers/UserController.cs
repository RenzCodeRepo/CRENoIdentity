using CRE.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                //if (result.Succeeded)
                //{
                //    // Check if the logged-in user has the 'Developer' role
                //    var user = await _userManager.FindByEmailAsync(model.Email);
                //    var roles = await _userManager.GetRolesAsync(user);

                //    if (roles.Contains("Developer"))
                //    {
                //        // You're logged in with the developer account
                //        return RedirectToAction("DeveloperDashboard", "Home");
                //    }
                //    else
                //    {
                //        // Non-developer login
                //        return RedirectToAction("UserDashboard", "Home");
                //    }
                //}

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }


    }
}
