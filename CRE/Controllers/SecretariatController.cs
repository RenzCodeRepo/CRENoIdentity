using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class SecretariatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
