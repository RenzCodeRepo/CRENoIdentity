using CRE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    public class EthicsEvaluationController : Controller
    {
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;

        public EthicsEvaluationController(IEthicsEvaluationServices ethicsEvaluationService)
        {
            _ethicsEvaluationServices = ethicsEvaluationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RespondToAssignment(int evaluationId, string response)
        {
            string status = response == "accept" ? "Accepted" : "Declined";
            await _ethicsEvaluationServices.UpdateEvaluationStatusAsync(evaluationId, status);
            return RedirectToAction("AssignedApplications");
        }
    }
}
