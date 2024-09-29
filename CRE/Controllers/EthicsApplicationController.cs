using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class EthicsApplicationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
