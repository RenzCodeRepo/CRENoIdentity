using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class InitialReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
