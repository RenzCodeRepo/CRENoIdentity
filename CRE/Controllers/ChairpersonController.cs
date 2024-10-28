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

            // Debugging: Log applications and evaluations
            foreach (var application in applications)
            {
                Console.WriteLine($"Application: {application.urecNo}");
                foreach (var eval in application.EthicsEvaluation)
                {
                    Console.WriteLine($"  Evaluation ID: {eval.evaluationId}, End Date: {eval.endDate}, Status: {eval.evaluationStatus}, " +
                                      $"Protocol Recommendation: {eval.ProtocolRecommendation}, Consent Recommendation: {eval.ConsentRecommendation}");
                }
            }

            var unassignedApplications = applications.Where(a =>
                !a.EthicsEvaluation.Any() || // No evaluations assigned
                (a.EthicsEvaluation.All(e => e.endDate == null) &&
                 a.EthicsEvaluation.All(e => e.EthicsEvaluator.declinedAssignment > 0)) || // All evaluations pending and declined
                !a.EthicsEvaluation.Any(e => e.evaluationStatus == "Assigned")); // Exclude those with assigned evaluations

            var underEvaluationApplications = applications.Where(a =>
                (a.EthicsEvaluation.Count == 3 &&
                 a.EthicsEvaluation.All(e => e.ProtocolRecommendation == "Pending" && e.ConsentRecommendation == "Pending")) || // Exactly three evaluations with Pending recommendations
                a.EthicsEvaluation.Any(e => e.endDate == null || e.evaluationStatus == "Assigned")); // At least one evaluation ongoing or assigned

            var evaluationResultApplications = applications.Where(a =>
                a.EthicsEvaluation.Count == 3 && a.EthicsEvaluation.All(e => e.endDate != null)); // All evaluations completed

            var viewModel = new ChairpersonApplicationsViewModel
            {
                UnassignedApplications = unassignedApplications.ToList(), // Convert to List
                UnderEvaluationApplications = underEvaluationApplications.ToList(), // Convert to List
                EvaluationResultApplications = evaluationResultApplications.ToList() // Convert to List
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> AssignEvaluators(string urecNo)
        {
            // Fetch application details and evaluators
            var viewModel = await _ethicsEvaluationServices.GetApplicationDetailsForEvaluationAsync(urecNo);

            // Get all available evaluators for the application’s field of study
            var availableEvaluators = await _ethicsEvaluationServices.GetAvailableEvaluatorsAsync(viewModel.EthicsApplication.fieldOfStudy);

            // Set both the available and recommended evaluators in the viewModel
            viewModel.AvailableEvaluators = availableEvaluators;
            viewModel.RecommendedEvaluators = availableEvaluators
                .OrderBy(e => e.pendingEval) // Order by least pending evaluations
                .Take(3) // Take the top 3 as recommended
                .ToList();

            // Pass the viewModel to the view
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignEvaluators(string urecNo, List<int> selectedEvaluatorIds)
        {
            if (string.IsNullOrEmpty(urecNo) || selectedEvaluatorIds == null || selectedEvaluatorIds.Count == 0)
            {
                // You can add some error handling here
                return BadRequest("Invalid parameters.");
            }

            // Assign evaluators asynchronously
            foreach (var evaluatorId in selectedEvaluatorIds)
            {
                await _ethicsEvaluationServices.AssignEvaluatorAsync(urecNo, evaluatorId);
            }

            // Redirect to a specific action after assignment
            return RedirectToAction("SelectApplication", new { urecNo = urecNo });
        }
    }
}
