using CRE.Data;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CRE.Controllers
{
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        // Login action
        
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    // If no user found, show error message
                    ModelState.AddModelError(string.Empty, "no user login attempt.");
                    return View(model);
                }

                // Sign in the user
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Any(role =>
                        role == UserRoles.Chief ||
                        role == UserRoles.Faculty ||
                        role == UserRoles.Evaluator ||
                        role == UserRoles.Secretariat ||
                        role == UserRoles.Chairperson))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("AccessDenied", "User");
                    }
                }

                // If we got here, the login attempt failed
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
