using CRE.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CRE.ViewModels;
using CRE.Services;
using CRE.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace CRE.Controllers
{
    public class ChiefController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly IAppUserServices _userServices;
        private readonly IReceiptInfoServices _receiptInfoServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IEthicsApplicationFormsServices _ethicsApplicationFormsServices;
        private readonly IInitialReviewServices _initialReviewServices;
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;
        private readonly UserManager<AppUser> _userManager;

        public ChiefController(
            IConfiguration configuration,
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            IAppUserServices userServices,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            ICoProponentServices coProponentServices,
            IEthicsApplicationFormsServices ethicsApplicationFormsServices,
            IInitialReviewServices initialReviewServices,
            IEthicsEvaluationServices ethicsEvaluationServices,
            UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _userServices = userServices;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _coProponentServices = coProponentServices;
            _ethicsApplicationFormsServices = ethicsApplicationFormsServices;
            _initialReviewServices = initialReviewServices;
            _ethicsEvaluationServices = ethicsEvaluationServices;
            _userManager = userManager; 
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Applications()
        {
            var approvedApplications = await _initialReviewServices.GetApprovedApplicationsAsync();

            var viewModel = new ApprovedApplicationListViewModel
            {
                ApprovedApplications = approvedApplications.Select(app => new ApprovedApplicationViewModel
                {
                    AppUser = app.AppUser,
                    Secretariat = app.Secretariat,
                    NonFundedResearchInfo = app.NonFundedResearchInfo,
                    CoProponent = app.CoProponent,
                    ReceiptInfo = app.ReceiptInfo,
                    Chairperson = app.Chairperson,
                    EthicsEvaluator = app.EthicsEvaluator,
                    EthicsApplication = app.EthicsApplication,
                    InitialReview = app.InitialReview,
                    EthicsApplicationForms = app.EthicsApplicationForms,
                    EthicsApplicationLog = app.EthicsApplicationLog
                }).ToList()
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Details(string urecNo)
        {
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo); // Assume this method fetches the application details

            if (applicationDetails == null)
            {
                return NotFound();
            }

            // Assuming you want to map this to AssignReviewTypeViewModel
            var viewModel = new AssignReviewTypeViewModel
            {
                AppUser = applicationDetails.AppUser,
                Secretariat = applicationDetails.Secretariat,
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponent,
                ReceiptInfo = applicationDetails.ReceiptInfo,
                Chairperson = applicationDetails.Chairperson,
                EthicsEvaluator = applicationDetails.EthicsEvaluator,
                EthicsApplication = applicationDetails.EthicsApplication,
                InitialReview = applicationDetails.InitialReview,
                EthicsApplicationForms = applicationDetails.EthicsApplicationForms,
                EthicsApplicationLog = applicationDetails.EthicsApplicationLog,
               
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReviewType(string ReviewType, string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("UREC No. is required.");
            }

            if (string.IsNullOrEmpty(ReviewType))
            {
                return BadRequest("Review Type is required.");
            }

            // Fetch the InitialReview based on the urecNo
            var initialReview = await _initialReviewServices.GetInitialReviewByUrecNoAsync(urecNo);

            if (initialReview != null)
            {
                // Update the review type
                initialReview.ReviewType = ReviewType;
                await _initialReviewServices.UpdateInitialReviewAsync(initialReview);

                // Log the review type assignment
                var logEntry = new EthicsApplicationLog
                {
                    urecNo = initialReview.urecNo,  // Assuming you have a link to EthicsApplication
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    changeDate = DateTime.Now,
                    status = "Review Type Assigned"

                };

                // Save the log entry (ensure you have a service or repository to handle logs)
                await _ethicsApplicationLogServices.AddLogAsync(logEntry);
            }

            // Redirect to the details page or any other page
            return RedirectToAction("Details", new { urecNo = urecNo });
        }

        public async Task<IActionResult> ExemptApplications()
        {
            var exemptApplications = await _initialReviewServices.GetExemptApplicationsAsync();

            var viewModel = new ExemptApplicationListViewModel
            {
                ExemptApplications = exemptApplications
            };

            return View(viewModel);
        }


        public async Task<IActionResult> EvaluateApplication(string urecNo)
        {
            // Fetch the exempt applications using the specified urecNo
            var exemptApplications = await _initialReviewServices.GetExemptApplicationsAsync();

            // Check if there are any exempt applications
            var applicationDetails = exemptApplications.FirstOrDefault(app => app.EthicsApplication.urecNo == urecNo);

            if (applicationDetails == null)
            {
                return NotFound(); // Return a 404 if the application is not found
            }

            // Get the current user (chief) from the User context
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId); // Assuming _userManager is injected

            // Create the view model from the retrieved application details
            var viewModel = new ChiefEvaluationViewModel
            {
                AppUser = applicationDetails.AppUser,
                Secretariat = applicationDetails.Secretariat,
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponent,
                ReceiptInfo = applicationDetails.ReceiptInfo,
                Chairperson = applicationDetails.Chairperson,
                EthicsEvaluator = applicationDetails.EthicsEvaluator,
                EthicsApplication = applicationDetails.EthicsApplication,
                InitialReview = applicationDetails.InitialReview,
                EthicsApplicationForms = applicationDetails.EthicsApplicationForms,
                EthicsApplicationLog = applicationDetails.EthicsApplicationLog,
                EthicsEvaluation = applicationDetails.EthicsEvaluation,
                ChiefName = currentUser != null ? $"{currentUser.fName} {currentUser.lName}" : "Not Assigned" // Set the ChiefName from the current user
            };

            return View(viewModel);
        }

    }
}
