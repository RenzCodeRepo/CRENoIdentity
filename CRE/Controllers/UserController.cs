using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
