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
            ApplicationDbContext context)
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

        [Authorize(Roles = "Secretariat, Chief")]
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


        public IActionResult Evaluations()
        {
            var viewModel = new ExemptApplicationListViewModel
            {
                ExemptApplications = GetExemptApplications(),
                EvaluatedExemptApplications = GetEvaluatedExemptApplications()
            };
            return View(viewModel);
        }

        private List<ChiefEvaluationViewModel> GetExemptApplications()
        {
            return _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.AppUser) // Include AppUser
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.CoProponent) // Include CoProponent
                .Where(a => a.InitialReview.ReviewType == "Exempt" && !a.EthicsEvaluation.Any())
                .Select(a => new ChiefEvaluationViewModel
                {
                    AppUser = a.User,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsApplication = a,
                    InitialReview = a.InitialReview,
                    ReceiptInfo = a.ReceiptInfo,
                    EthicsApplicationForms = a.EthicsApplicationForms,
                    EthicsApplicationLog = a.EthicsApplicationLog
                }).ToList();
        }


        private List<EvaluatedExemptApplication> GetEvaluatedExemptApplications()
        {
            return _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Include NonFundedResearchInfo
                    .ThenInclude(n => n.AppUser) // Include AppUser inside NonFundedResearchInfo
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.CoProponent) // Include CoProponent inside NonFundedResearchInfo
                .Where(a => a.InitialReview.ReviewType == "Exempt" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedExemptApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.FirstOrDefault(), // Get the first evaluation
                    InitialReview = a.InitialReview,
                    User = a.User,
                    EthicsApplicationLog = a.EthicsApplicationLog
                }).ToList();
        }


        // Action to display evaluated exempt applications
        public IActionResult EvaluatedExemptApplications()
        {
            var viewModel = new EvaluatedExemptApplicationListViewModel
            {
                EvaluatedExemptApplications = GetEvaluatedExemptApplications()
            };
            return View(viewModel);
        }

        public IActionResult EvaluatedExpeditedApplications()
        {
            var viewModel = new EvaluatedExpeditedApplicationListViewModel
            {
                EvaluatedExpeditedApplications = GetEvaluatedExpeditedApplications()
            };
            return View(viewModel);
        }

        public IActionResult EvaluatedFullReviewApplications()
        {
            var viewModel = new EvaluatedFullReviewApplicationListViewModel
            {
                EvaluatedFullReviewApplications = GetEvaluatedFullReviewApplications()
            };
            return View(viewModel);
        }
        // Get evaluated expedited applications method
        private List<EvaluatedExpeditedApplication> GetEvaluatedExpeditedApplications()
        {
            return _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Ensure this is included
                .Where(a => a.InitialReview.ReviewType == "Expedited" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedExpeditedApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.ToList(), // Include all evaluations
                    InitialReview = a.InitialReview,
                    User = a.User,
                    EthicsApplicationLog = a.EthicsApplicationLog
                }).ToList();
        }

        // Get evaluated full review applications method
        private List<EvaluatedFullReviewApplication> GetEvaluatedFullReviewApplications()
        {
            return _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Ensure this is included
                .Where(a => a.InitialReview.ReviewType == "Full Review" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedFullReviewApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.ToList(), // Include all evaluations
                    InitialReview = a.InitialReview,
                    User = a.User,
                    EthicsApplicationLog = a.EthicsApplicationLog
                }).ToList();
        }
        public IActionResult FilteredApplications()
        {
            var model = new ApplicationListViewModel
            {
                ApplicationsApprovedForEvaluation = GetApplicationsByInitialReviewType("Pending"),
                ExemptApplications = GetApplicationsBySubmitReviewType("Exempt"),
                ExpeditedApplications = GetApplicationsBySubmitReviewType("Expedited"),
                FullReviewApplications = GetApplicationsBySubmitReviewType("Full Review"),
                AllApplications = GetAllApplications()
            };

            return View(model);
        }
        private List<ApplicationViewModel> GetApplicationsByInitialReviewType(string reviewType)
        {
            return _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Ensure this is included
                .Where(a => a.InitialReview.ReviewType == reviewType)
                .Select(a => new ApplicationViewModel
                {
                    UrecNo = a.urecNo,
                    Title = a.NonFundedResearchInfo.title,
                    // Get the latest status from the application logs
                    Status = a.EthicsApplicationLog
                                .OrderByDescending(log => log.changeDate) // Assuming 'Date' is the property indicating when the log was created
                                .Select(log => log.status)
                                .FirstOrDefault(),
                    SubmissionDate = a.submissionDate.ToDateTime(TimeOnly.MinValue),
                }).ToList();
        }

        private List<ApplicationViewModel> GetApplicationsBySubmitReviewType(string reviewType)
        {
            return _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Ensure this is included
                .Where(a => a.InitialReview.ReviewType == reviewType)
                .Select(a => new ApplicationViewModel
                {
                    UrecNo = a.urecNo,
                    Title = a.NonFundedResearchInfo.title,
                    // Get the latest status from the application logs
                    Status = a.EthicsApplicationLog
                                .OrderByDescending(log => log.changeDate) // Assuming 'Date' is the property indicating when the log was created
                                .Select(log => log.status)
                                .FirstOrDefault(),
                    SubmissionDate = a.submissionDate.ToDateTime(TimeOnly.MinValue),
                }).ToList();
        }

        private List<ApplicationViewModel> GetAllApplications()
        {
            return _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Ensure this is included
                .Select(a => new ApplicationViewModel
                {
                    UrecNo = a.urecNo,
                    Title = a.NonFundedResearchInfo.title,
                    // Get the latest status from the application logs
                    Status = a.EthicsApplicationLog
                                .OrderByDescending(log => log.changeDate) // Assuming 'Date' is the property indicating when the log was created
                                .Select(log => log.status)
                                .FirstOrDefault(),
                    SubmissionDate = a.submissionDate.ToDateTime(TimeOnly.MinValue),
                }).ToList();
        }

    }
}
