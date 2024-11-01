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
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Roles = "Chairperson")]
        [HttpGet]
        public async Task<IActionResult> SelectApplication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ethicsApplications = await _chairpersonServices.GetApplicationsByFieldOfStudyAsync(userId);
            var evaluatorNames = await _chairpersonServices.GetEvaluatorNamesAsync(ethicsApplications);
            var unassignedApplications = await _chairpersonServices.GetUnassignedApplicationsAsync(ethicsApplications);
            var underEvaluationApplications = await _chairpersonServices.GetUnderEvaluationApplicationsAsync(ethicsApplications);
            var evaluationResultApplications = await _chairpersonServices.GetEvaluationResultApplicationsAsync(ethicsApplications);
            var nonFundedResearchInfos = await _nonFundedResearchInfoServices.GetNonFundedResearchInfosAsync(ethicsApplications.Select(a => a.urecNo).ToList());

            var viewModel = new ChairpersonApplicationsViewModel
            {
                UnassignedApplications = unassignedApplications.ToList(),
                UnderEvaluationApplications = underEvaluationApplications.ToList(),
                EvaluationResultApplications = evaluationResultApplications.ToList(),
                ApplicationEvaluatorNames = evaluatorNames,
                NonFundedResearchInfo = nonFundedResearchInfos
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Chairperson")]
        [HttpGet]
        public async Task<IActionResult> AssignEvaluators(string urecNo)
        {
            // Fetch application details
            var viewModel = await _ethicsEvaluationServices.GetApplicationDetailsForEvaluationAsync(urecNo);
            string requiredFieldOfStudy = viewModel.EthicsApplication.fieldOfStudy;

            // Get applicant's full name
            var applicantUser = viewModel.EthicsApplication.User;
            string applicantFirstName = applicantUser?.fName;
            string applicantMiddleName = applicantUser?.mName;
            string applicantLastName = applicantUser?.lName;

            // Retrieve all evaluators with expertise data
            var allEvaluators = await _ethicsEvaluationServices.GetAllEvaluatorsAsync();

            // Filter available and recommended evaluators using service methods
            viewModel.AvailableEvaluators = (await _ethicsEvaluationServices
                .GetAvailableEvaluatorsAsync(allEvaluators, applicantFirstName, applicantMiddleName, applicantLastName))
                .ToList();

            viewModel.RecommendedEvaluators = (await _ethicsEvaluationServices
                .GetRecommendedEvaluatorsAsync(allEvaluators, requiredFieldOfStudy, applicantFirstName, applicantMiddleName, applicantLastName))
                .ToList();

            // Pass the viewModel to the view
            return View(viewModel);
        }

        [Authorize(Roles = "Chairperson")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
