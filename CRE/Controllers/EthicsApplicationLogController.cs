using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class EthicsApplicationLogController : Controller
    {
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        public EthicsApplicationLogController(IEthicsApplicationLogServices ethicsApplicationLogServices)
        {
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TrackApplication(string urecNo)
        {
            var logs = await _ethicsApplicationLogServices.GetLogsByUrecNoAsync(urecNo);

            if (logs == null || !logs.Any())
            {
                // Handle case where no logs are found (optional)
                return View(new List<EthicsApplicationLog>()); // Pass an empty list if no logs found
            }

            return View(logs); // Pass the list of logs to the view
        }
    }
}
