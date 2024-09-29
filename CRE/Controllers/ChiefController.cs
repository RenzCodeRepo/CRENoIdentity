using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class ChiefController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
