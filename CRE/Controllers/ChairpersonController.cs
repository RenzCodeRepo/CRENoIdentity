using CRE.Interfaces;
using CRE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using CRE.Services;
using CRE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CRE.Data;

namespace CRE.Controllers
{
    public class ChairpersonController : Controller
    {

        private readonly IChairpersonServices _chairpersonService;
        private readonly IEthicsEvaluationServices _ethicsEvaluationService;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;


        public ChairpersonController(IChairpersonServices chairpersonService, IEthicsEvaluationServices ethicsEvaluationService, INonFundedResearchInfoServices nonFundedResearchInfoServices)
        {
            _chairpersonService = chairpersonService;
            _ethicsEvaluationService = ethicsEvaluationService;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
        }

        public async Task<IActionResult> SelectApplication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _chairpersonService.GetApplicationsByFieldOfStudyAsync(userId);

            var unassignedApplications = applications.Where(a =>
                !a.EthicsEvaluation.Any() // No evaluators assigned
                || a.EthicsEvaluation.All(e => e.endDate == null && e.EthicsEvaluator.declinedAssignment > 0));

            var underEvaluationApplications = applications.Where(a =>
                a.EthicsEvaluation.Any(e => e.endDate == null && e.EthicsEvaluator.declinedAssignment == 0));

            var evaluationResultApplications = applications.Where(a =>
                a.EthicsEvaluation.Count == 3 && a.EthicsEvaluation.All(e => e.endDate != null));

            var viewModel = new ChairpersonApplicationsViewModel
            {
                UnassignedApplications = unassignedApplications,
                UnderEvaluationApplications = underEvaluationApplications,
                EvaluationResultApplications = evaluationResultApplications
            };

            return View(viewModel);
        }
        public async Task<IActionResult> AssignEvaluators(string urecNo)
        {
            var application = await _chairpersonService.GetApplicationAsync(urecNo);
            var availableEvaluators = await _ethicsEvaluationService.GetAvailableEvaluatorsAsync(application.fieldOfStudy);
            var nonFundedResearchInfo = await _nonFundedResearchInfoServices.GetNonFundedResearchByUrecNoAsync(urecNo);


            var viewModel = new AssignEvaluatorsViewModel
            {
                EthicsApplication = application,
                AvailableEvaluators = availableEvaluators,
                NonFundedResearchInfo = nonFundedResearchInfo
            };

            return View(viewModel); 
        }

        [HttpPost]
        public async Task<IActionResult> AssignEvaluators(string urecNo, List<int> evaluatorIds)
        {
            foreach (var evaluatorId in evaluatorIds)
            {
                await _ethicsEvaluationService.AssignEvaluatorAsync(urecNo, evaluatorId);
            }

            return RedirectToAction("ApplicationDetails", new { urecNo = urecNo });
        }
    }
}
