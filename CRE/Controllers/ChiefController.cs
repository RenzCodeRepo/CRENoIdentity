using CRE.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CRE.ViewModels;
using CRE.Services;
using CRE.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
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
            var allExemptApplications = await _initialReviewServices.GetExemptApplicationsAsync();

            var evaluatedExemptApplications = allExemptApplications
                .Where(app => app.EthicsApplicationLog != null &&
                              app.EthicsApplicationLog.Any(log => log.status == "Evaluated"))
                .Select(async app => new EvaluatedExemptApplication
                {
                    EthicsApplication = app.EthicsApplication,
                    NonFundedResearchInfo = app.NonFundedResearchInfo,
                    EthicsApplicationLog = app.EthicsApplicationLog,
                    EthicsEvaluation = await _initialReviewServices.GetEthicsEvaluationAsync(app.EthicsApplication.urecNo)
                })
                .Select(task => task.Result) // Resolve the Task to avoid async issues in LINQ
                .ToList();

            var exemptApplications = allExemptApplications
                .Where(app => !evaluatedExemptApplications.Any(evaluated => evaluated.EthicsApplication.urecNo == app.EthicsApplication.urecNo))
                .ToList();

            var viewModel = new ExemptApplicationListViewModel
            {
                ExemptApplications = exemptApplications,
                EvaluatedExemptApplications = evaluatedExemptApplications
            };

            return View(viewModel);
        }




        public async Task<IActionResult> EvaluateApplication(string urecNo)
        {
            // Fetch the application details using the same service method as in Details
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);

            if (applicationDetails == null)
            {
                return NotFound();
            }

            // Create the view model
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
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EvaluateApplication(ChiefEvaluationViewModel model)
        {
            ModelState.Remove("EthicsApplication.User");
            ModelState.Remove("EthicsApplication.userId");
            ModelState.Remove("EthicsApplication.fieldOfStudy");

            if (!ModelState.IsValid)
            {
                // Return the same view with the model to show validation errors
                return View(model);
            }
            // Retrieve the current user ID from the claims
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the current user
            var currentUser = await _userManager.Users
                .Include(u => u.Chief) // Include the Chief navigation property
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser?.Chief == null)
            {
                // Handle the case where the chief is not found
                return NotFound("Chief not found for the current user.");
            }

            // Access the chiefId
            var chiefId = currentUser.Chief.chiefId;

            // Create the EthicsEvaluation entity
            var ethicsEvaluation = new EthicsEvaluation
            {
                urecNo = model.EthicsApplication.urecNo, // Ensure this property exists in your model
                chiefId = chiefId, // Use the chiefId from the retrieved Chief
                ProtocolRecommendation = model.EthicsEvaluation.ProtocolRecommendation,
                ProtocolRemarks = model.EthicsEvaluation.ProtocolRemarks,
                ConsentRecommendation = model.EthicsEvaluation.ConsentRecommendation,
                ConsentRemarks = model.EthicsEvaluation.ConsentRemarks,
                startDate = DateOnly.FromDateTime(DateTime.Today),
                endDate = DateOnly.FromDateTime(DateTime.Today),
                ProtocolReviewSheet = model.ProtocolReviewSheet != null ? await GetFileContentAsync(model.ProtocolReviewSheet) : null,
                InformedConsentForm = model.InformedConsentForm != null ? await GetFileContentAsync(model.InformedConsentForm) : null,
            };
            
            // Save the evaluation to the database
            await _ethicsEvaluationServices.SaveEvaluationAsync(ethicsEvaluation);

            // Add an entry to the EthicsApplicationLog
            // Create a log entry to record the evaluation submission
            var applicationLog = new EthicsApplicationLog
            {
                urecNo = model.EthicsApplication?.urecNo, // Link to the evaluated application
                userId = currentUserId, // ID of the user performing the evaluation
                status = "Evaluated", // Status of the application update
                changeDate = DateTime.Now, // Current date and time
                comments = "The application has been evaluated and marked as submitted."
            };
            // Save the log entry to the database
            await _ethicsApplicationLogServices.AddLogAsync(applicationLog);
            return RedirectToAction("ExemptApplications", new { success = true });

        }
        private async Task<byte[]> GetFileContentAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<IActionResult> ViewEvaluationDetails(string urecNo, int evaluationId)
        {
            var evaluatedApplication = await _ethicsEvaluationServices.GetEvaluationDetailsAsync(urecNo, evaluationId);
            if (evaluatedApplication == null)
            {
                return NotFound();
            }
            return View(evaluatedApplication);
        }


        [HttpGet]
        public async Task<IActionResult> ViewFile(string fileType, string urecNo, int evaluationId)
        {
            // Fetch the EthicsEvaluation based on urecNo and evaluationId
            var ethicsEvaluation = await _ethicsEvaluationServices.GetEvaluationByUrecNoAndIdAsync(urecNo, evaluationId);

            if (ethicsEvaluation == null)
            {
                return NotFound();
            }

            byte[] fileData = null;
            string contentType = "";

            if (fileType == "ProtocolReviewSheet")
            {
                fileData = ethicsEvaluation.ProtocolReviewSheet;
                contentType = "application/pdf"; // Ensure the content type matches your file type
            }
            else if (fileType == "InformedConsentForm")
            {
                fileData = ethicsEvaluation.InformedConsentForm;
                contentType = "application/pdf"; // Ensure the content type matches your file type
            }

            if (fileData == null)
            {
                return NotFound();
            }

            return File(fileData, contentType);
        }
    }
}
