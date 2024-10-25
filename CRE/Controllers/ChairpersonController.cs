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

        private readonly IChairpersonServices _chairpersonServices;
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IAppUserServices _userServices;


        public ChairpersonController(IChairpersonServices chairpersonServices,
            IEthicsEvaluationServices ethicsEvaluationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            ICoProponentServices coProponentServices,
            IAppUserServices userServices)
        {
            _chairpersonServices = chairpersonServices;
            _ethicsEvaluationServices = ethicsEvaluationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _coProponentServices = coProponentServices;
            _userServices = userServices;
        }

        public async Task<IActionResult> SelectApplication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _chairpersonServices.GetApplicationsByFieldOfStudyAsync(userId);

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
        [HttpGet]
        public async Task<IActionResult> AssignEvaluators(string urecNo)
        {
            var viewModel = await _ethicsEvaluationServices.GetApplicationDetailsForEvaluationAsync(urecNo);

            var availableEvaluators = await _ethicsEvaluationServices.GetAvailableEvaluatorsAsync(viewModel.EthicsApplication.fieldOfStudy);

            // Set the available evaluators
            viewModel.AvailableEvaluators = availableEvaluators;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignEvaluators(string urecNo, List<int> evaluatorIds)
        {
            foreach (var evaluatorId in evaluatorIds)
            {
                await _ethicsEvaluationServices.AssignEvaluatorAsync(urecNo, evaluatorId);
            }

            return RedirectToAction("ApplicationDetails", new { urecNo = urecNo });
        }
    }
}
