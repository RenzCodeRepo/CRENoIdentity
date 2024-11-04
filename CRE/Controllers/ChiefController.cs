using CRE.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CRE.ViewModels;
using CRE.Services;
using CRE.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using CRE.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DocumentFormat.OpenXml.Wordprocessing;
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
        private readonly IEthicsClearanceServices _ethicsClearanceServices;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

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
            UserManager<AppUser> userManager,
            ApplicationDbContext context,
            IEthicsClearanceServices ethicsClearanceServices)
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
            _context = context;
            _ethicsClearanceServices = ethicsClearanceServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(string urecNo)
        {
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);
            if (applicationDetails == null)
            {
                return NotFound();
            }
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
                EthicsApplicationLog = applicationDetails.EthicsApplicationLog
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReviewType(string reviewType, string urecNo)
        {
            if (string.IsNullOrEmpty(reviewType) || string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Required fields (UREC No. and Review Type) are missing.");
            }

            var initialReview = await _initialReviewServices.GetInitialReviewByUrecNoAsync(urecNo);

            if (initialReview != null)
            {
                initialReview.ReviewType = reviewType;
                await _initialReviewServices.UpdateInitialReviewAsync(initialReview);

                var logEntry = new EthicsApplicationLog
                {
                    urecNo = initialReview.urecNo,
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    changeDate = DateTime.Now,
                    status = "Review Type Assigned"
                };

                await _ethicsApplicationLogServices.AddLogAsync(logEntry);
            }

            return RedirectToAction("FilteredApplications");
        }



        [Authorize(Roles = "Chief")]
        [HttpGet]
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



        [Authorize(Roles = "Chief")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EvaluateApplication(ChiefEvaluationViewModel model)
        {
            ModelState.Remove("EthicsEvaluation.EthicsEvaluator");
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
                evaluationStatus = "Evaluated",
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
            return RedirectToAction("Evaluations", new { success = true });

        }
        private async Task<byte[]> GetFileContentAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


        [Authorize(Roles = "Chief")]
        [HttpGet]
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


        public async Task<IActionResult> Evaluations()
        {
            var viewModel = new ApplicationEvaluationViewModel
            {
                ExemptApplications = await _ethicsEvaluationServices.GetExemptApplicationsAsync(),
                EvaluatedExemptApplications = await _ethicsEvaluationServices.GetEvaluatedExemptApplicationsAsync(),
                EvaluatedExpeditedApplications = await _ethicsEvaluationServices.GetEvaluatedExpeditedApplicationsAsync(),
                EvaluatedFullReviewApplications = await _ethicsEvaluationServices.GetEvaluatedFullReviewApplicationsAsync(),
                PendingIssuance = await _ethicsEvaluationServices.GetPendingApplicationsForIssuanceAsync() // Add this line
            };

            return View(viewModel);
        }

        public async Task<IActionResult> FilteredApplications()
        {
            var model = new ApplicationListViewModel
            {
                ApplicationsApprovedForEvaluation = await _ethicsApplicationServices.GetApplicationsByInitialReviewTypeAsync("Pending"),
                ExemptApplications = await _ethicsApplicationServices.GetApplicationsBySubmitReviewTypeAsync("Exempt"),
                ExpeditedApplications = await _ethicsApplicationServices.GetApplicationsBySubmitReviewTypeAsync("Expedited"),
                FullReviewApplications = await _ethicsApplicationServices.GetApplicationsBySubmitReviewTypeAsync("Full Review"),
                AllApplications = await _ethicsApplicationServices.GetAllApplicationViewModelsAsync(),
                PendingIssuance = await _ethicsEvaluationServices.GetPendingApplicationsForIssuanceAsync() // Added line
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> IssueApplication(string urecNo)
        {
            // Call the service method
            var viewModel = await _ethicsApplicationServices.GetEvaluationDetailsAsync(urecNo);

            if (viewModel == null)
            {
                return NotFound(); // Handle application not found
            }

            return View(viewModel); // Return the view with the populated model
        }
        [HttpPost]
        public async Task<IActionResult> IssueApplication(EthicsApplication viewModel, IFormFile uploadedFile, string applicationDecision, string remarks)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (applicationDecision == "Approve" && uploadedFile != null)
            {
                var ethicsClearance = new EthicsClearance
                {
                    urecNo = viewModel.urecNo, // Link urecNo to EthicsClearance
                    issuedDate = DateOnly.FromDateTime(DateTime.Now), // Set issued date as DateOnly
                    expirationDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)) // Set expiration date one year from now as DateOnly
                };  

                var success = await _ethicsClearanceServices.IssueEthicsClearanceAsync(ethicsClearance, uploadedFile, remarks, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Ethics clearance issued successfully!";
                    return RedirectToAction("FilteredApplications");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select 'Approve' and upload a PDF file.");
            }

            return View(viewModel);
        }

    }
}
