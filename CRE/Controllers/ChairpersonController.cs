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


        public ChairpersonController(IChairpersonServices chairpersonService, IEthicsEvaluationServices ethicsEvaluationService)
        {
            _chairpersonService = chairpersonService;
            _ethicsEvaluationService = ethicsEvaluationService;
        }

        public async Task<IActionResult> SelectApplication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var applications = await _chairpersonService.GetApplicationsByFieldOfStudyAsync(userId);

            var viewModel = new ChairpersonApplicationsViewModel
            {
                EthicsApplications = applications,
            };

            return View(viewModel);
        }
        public async Task<IActionResult> AssignEvaluators(string urecNo)
        {
            var application = await _chairpersonService.GetApplicationAsync(urecNo);
            var availableEvaluators = await _ethicsEvaluationService.GetAvailableEvaluatorsAsync(application.fieldOfStudy);

            var viewModel = new AssignEvaluatorsViewModel
            {
                EthicsApplication = application,
                AvailableEvaluators = availableEvaluators
            };

            return View(viewModel); // Passes the application and available evaluators to the view
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
