using CRE.Interfaces;
using CRE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRE.Controllers
{
    public class EthicsEvaluatorController : Controller
    {
        private readonly IEthicsEvaluationServices _ethicsEvaluationService;
        public EthicsEvaluatorController(IEthicsEvaluationServices ethicsEvaluationService)
        {
            _ethicsEvaluationService = ethicsEvaluationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RespondToAssignment(int evaluationId, string response)
        {
            string status = response == "accept" ? "Accepted" : "Declined";
            await _ethicsEvaluationService.UpdateEvaluationStatusAsync(evaluationId, status);

            return RedirectToAction("AssignedApplications");
        }
    }
}
