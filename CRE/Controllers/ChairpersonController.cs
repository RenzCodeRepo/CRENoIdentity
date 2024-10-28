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
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;


        public ChairpersonController(IChairpersonServices chairpersonServices,
            IEthicsEvaluationServices ethicsEvaluationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            ICoProponentServices coProponentServices,
            IAppUserServices userServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices)
        {
            _chairpersonServices = chairpersonServices;
            _ethicsEvaluationServices = ethicsEvaluationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _coProponentServices = coProponentServices;
            _userServices = userServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
        }

        public async Task<IActionResult> SelectApplication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _chairpersonServices.GetApplicationsByFieldOfStudyAsync(userId);

            var applicationEvaluatorNames = new Dictionary<string, List<string>>();

            foreach (var application in applications)
            {
                // Initialize evaluator names for this application
                var evaluatorNames = new List<string>();

                // Retrieve the evaluators for the application
                foreach (var evaluation in application.EthicsEvaluation)
                {
                    // Ensure the evaluator exists and retrieve their full name
                    var evaluator = evaluation.EthicsEvaluator; // Assuming EthicsEvaluator is a navigation property
                    if (evaluator != null)
                    {
                        // Make sure to include Faculty and User in your query to avoid null reference
                        var faculty = evaluator.Faculty;
                        if (faculty != null)
                        {
                            var user = faculty.User; // Assuming User is a navigation property in Faculty
                            if (user != null)
                            {
                                var userName = $"{user.fName} {user.lName}"; // Construct full name
                                evaluatorNames.Add(userName);
                            }
                        }
                    }
                }

                // Add evaluator names to the dictionary with the application urecNo as the key
                applicationEvaluatorNames[application.urecNo] = evaluatorNames;
            }

            var unassignedApplications = applications.Where(a =>
                !a.EthicsEvaluation.Any() || // No evaluations assigned
                (a.EthicsEvaluation.All(e => e.endDate == null) &&
                 a.EthicsEvaluation.Any(e => e.EthicsEvaluator.declinedAssignment > 0)) || // All evaluations pending and declined
                !a.EthicsEvaluation.Any(e => e.evaluationStatus == "Assigned")) // Exclude those with assigned evaluations
                .Where(a => a.InitialReview != null && // Ensure InitialReview is not null
                        (a.InitialReview.ReviewType == "Expedited" || a.InitialReview.ReviewType == "Full Review")); // Check for review type

            var underEvaluationApplications = applications.Where(a =>
                (a.EthicsEvaluation.Count == 3 &&
                 a.EthicsEvaluation.All(e => e.ProtocolRecommendation == "Pending" && e.ConsentRecommendation == "Pending")) || // Exactly three evaluations with Pending recommendations
                a.EthicsEvaluation.Any(e => e.endDate == null || e.evaluationStatus == "Assigned")); // At least one evaluation ongoing or assigned

            var evaluationResultApplications = applications.Where(a =>
                a.EthicsEvaluation.Count == 3 && a.EthicsEvaluation.All(e => e.endDate != null)); // All evaluations completed

            var nonFundedResearchInfos = await _nonFundedResearchInfoServices.GetNonFundedResearchInfosAsync(applications.Select(a => a.urecNo).ToList());
            var viewModel = new ChairpersonApplicationsViewModel
            {
                UnassignedApplications = unassignedApplications.ToList(), // Convert to List
                UnderEvaluationApplications = underEvaluationApplications.ToList(), // Convert to List
                EvaluationResultApplications = evaluationResultApplications.ToList(), // Convert to List
                ApplicationEvaluatorNames = applicationEvaluatorNames, // Assign evaluator names to the ViewModel
                NonFundedResearchInfo = nonFundedResearchInfos // Assign NonFundedResearchInfo to the ViewModel
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
            // Validate input parameters
            if (string.IsNullOrEmpty(urecNo) || selectedEvaluatorIds == null || selectedEvaluatorIds.Count == 0)
            {
                return BadRequest("Invalid parameters.");
            }

            // Get the user ID of the logged-in chairperson
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Assign evaluators asynchronously
            foreach (var evaluatorId in selectedEvaluatorIds)
            {
                await _ethicsEvaluationServices.AssignEvaluatorAsync(urecNo, evaluatorId);
            }

            // Create a new log entry
            var logEntry = new EthicsApplicationLog
            {
                urecNo = urecNo,
                userId = userId,
                status = "Evaluators Assigned",
                changeDate = DateTime.UtcNow
            };

            // Save the log entry using the EthicsApplicationLogServices
            await _ethicsApplicationLogServices.AddLogAsync(logEntry); // Ensure you have this method in your service

            // Redirect to a specific action after assignment
            return RedirectToAction("SelectApplication", new { urecNo = urecNo });
        }


    }
}
