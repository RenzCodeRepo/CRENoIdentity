using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class NonFundedResearchInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
