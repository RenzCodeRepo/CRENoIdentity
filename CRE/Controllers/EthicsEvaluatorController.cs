using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using CRE.ViewModels;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRE.Controllers
{
    public class EthicsEvaluatorController : Controller
    {
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;
        private readonly IInitialReviewServices _initialReviewServices;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        public EthicsEvaluatorController(IEthicsEvaluationServices ethicsEvaluationServices, IInitialReviewServices initialReviewServices,
            UserManager<AppUser> userManager, IEthicsApplicationLogServices ethicsApplicationLogServices, IEthicsApplicationServices ethicsApplicationServices)
        {
            _ethicsEvaluationServices = ethicsEvaluationServices;
            _initialReviewServices = initialReviewServices;
            _userManager = userManager;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _ethicsApplicationServices = ethicsApplicationServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EvaluatorView()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var evaluator = await _ethicsEvaluationServices.GetEvaluatorByUserIdAsync(userId);
            if (evaluator == null)
            {
                // Handle the case when the evaluator is not found
                return NotFound("Evaluator not found");
            }
            // Extract the evaluatorId from the evaluator object
            var evaluatorId = evaluator.ethicsEvaluatorId; // Adjust according to your model

            var viewModel = new TabbedEvaluationViewModel
            {
                EvaluationAssignments = await _ethicsEvaluationServices.GetAssignedEvaluationsAsync(evaluatorId),
                ToBeEvaluated = await _ethicsEvaluationServices.GetAcceptedEvaluationsAsync(evaluatorId),
                Evaluated = await _ethicsEvaluationServices.GetCompletedEvaluationsAsync(evaluatorId),
                DeclinedEvaluations = await _ethicsEvaluationServices.GetDeclinedEvaluationsAsync(evaluatorId)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> RespondToAssignment(string id, int evaluationId)
        {
            // Fetch the application details based on urecNo and evaluationId
            var viewModel = await _ethicsEvaluationServices.GetEvaluationDetailsWithUrecNoAsync(id, evaluationId);

            // Check if evaluation details were found
            if (viewModel == null)
            {
                return NotFound(); // Return a 404 if not found
            }

            // Pass the view model to the view
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> RespondToAssignment(string acceptanceStatus, string urecNo, int evalId, string? reasonForDecline)
        {
            // Retrieve the current user ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the current user with associated EthicsEvaluator
            var currentUser = await _userManager.Users
                .Include(u => u.Faculty.EthicsEvaluator)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser?.Faculty.EthicsEvaluator == null)
            {
                return NotFound("Evaluator not found for the current user.");
            }

            // Extract the evaluator ID
            var ethicsEvaluatorId = currentUser.Faculty.EthicsEvaluator.ethicsEvaluatorId;

            // Check if an evaluation already exists for this urecNo and evaluator
            var existingEvaluation = await _ethicsEvaluationServices.GetEvaluationByUrecNoAndEvaluatorIdAsync(urecNo, ethicsEvaluatorId);

            if (existingEvaluation != null)
            {
                if (acceptanceStatus == "Declined")
                {
                    // Mark application as "Unassigned"
                    await _ethicsApplicationServices.UpdateApplicationStatusAsync(existingEvaluation.evaluationId, urecNo, "Unassigned");
                    await _ethicsEvaluationServices.UpdateEvaluationStatusAsync(existingEvaluation.evaluationId, "Declined", reasonForDecline, ethicsEvaluatorId);

                    await _ethicsEvaluationServices.IncrementDeclinedAssignmentCountAsync(ethicsEvaluatorId);
                    
                }
                else
                {
                    // Update the existing evaluation's status for accepted applications
                    await _ethicsEvaluationServices.UpdateEvaluationStatusAsync(existingEvaluation.evaluationId, "Accepted", null, ethicsEvaluatorId);
                }
            }
            else
            {
                // Create a new evaluation entry if none exists
                var newEvaluation = new EthicsEvaluation
                {
                    evaluationStatus = acceptanceStatus,
                    startDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    ethicsEvaluatorId = ethicsEvaluatorId,
                    urecNo = urecNo,
                    reasonForDecline = acceptanceStatus == "Declined" ? reasonForDecline : null // Set reason for decline if applicable
                };

                await _ethicsEvaluationServices.CreateEvaluationAsync(newEvaluation);

                // If declined, update application status to "Unassigned"
                if (acceptanceStatus == "Declined")
                {
                    await _ethicsApplicationServices.UpdateApplicationStatusAsync(newEvaluation.evaluationId, urecNo, "Unassigned"); // Ensure application is marked unassigned
                }
            }

            // Fetch application details for the view model (to show after response)
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);

            // Create and populate the view model with application details
            var viewModel = new EvaluationDetailsViewModel
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

            // Add a success message
            TempData["SuccessMessage"] = "Evaluation has been successfully processed.";

            // Redirect to the EvaluatorView with success
            return RedirectToAction("EvaluatorView", new { success = true });
        }




        public async Task<IActionResult> EvaluationDetails(string id)
        {
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(id);


            // Check if evaluation details were found
            if (applicationDetails == null)
            {
                return NotFound(); // Return a 404 if not found
            }
            // Create the view model
            var viewModel = new EvaluationDetailsViewModel
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
            // Pass the details to the view
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EvaluationDetails(EvaluationDetailsViewModel model)
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
                .Include(u => u.Faculty.EthicsEvaluator)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser?.Faculty.EthicsEvaluator == null)
            {
                return NotFound("Evaluator not found for the current user.");
            }

            var ethicsEvaluatorId = currentUser.Faculty.EthicsEvaluator.ethicsEvaluatorId;

            // Check if the evaluation already exists
            var existingEvaluation = await _ethicsEvaluationServices.GetEvaluationByUrecNoAndEvaluatorIdAsync(model.EthicsApplication.urecNo, ethicsEvaluatorId);

            if (existingEvaluation != null)
            {
                // Update the existing evaluation
                existingEvaluation.evaluationStatus = "Evaluated";
                existingEvaluation.ProtocolRecommendation = model.EthicsEvaluation.ProtocolRecommendation;
                existingEvaluation.ProtocolRemarks = model.EthicsEvaluation.ProtocolRemarks;
                existingEvaluation.ConsentRecommendation = model.EthicsEvaluation.ConsentRecommendation;
                existingEvaluation.ConsentRemarks = model.EthicsEvaluation.ConsentRemarks;
                existingEvaluation.endDate = DateOnly.FromDateTime(DateTime.Today);

                if (model.ProtocolReviewSheet != null)
                {
                    existingEvaluation.ProtocolReviewSheet = await GetFileContentAsync(model.ProtocolReviewSheet);
                }

                if (model.InformedConsentForm != null)
                {
                    existingEvaluation.InformedConsentForm = await GetFileContentAsync(model.InformedConsentForm);
                }

                // Save the updated evaluation to the database
                await _ethicsEvaluationServices.UpdateEvaluationAsync(existingEvaluation);
            }
            else
            {
                return NotFound("No existing evaluation found for the provided urecNo and evaluator.");
            }

            // Add an entry to the EthicsApplicationLog
            var applicationLog = new EthicsApplicationLog
            {
                urecNo = model.EthicsApplication.urecNo,
                userId = currentUserId,
                status = "Evaluated",
                changeDate = DateTime.Now,
                comments = "The application has been evaluated and marked as submitted."
            };

            await _ethicsApplicationLogServices.AddLogAsync(applicationLog);

            return RedirectToAction("EvaluatorView", new { success = true });
        }

        private async Task<byte[]> GetFileContentAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public async Task<IActionResult> Evaluated(string urecNo, int evaluationId)
        {
            var evaluatedApplication = await _ethicsEvaluationServices.GetEvaluationAndEvaluatorDetailsAsync(urecNo, evaluationId);
            if (evaluatedApplication == null)
            {
                return NotFound();
            }

            // Map the evaluatedApplication to EvaluationDetailsViewModel
            var evaluationDetailsViewModel = new EvaluationDetailsViewModel
            {
                EthicsApplication = evaluatedApplication.EthicsApplication,
                NonFundedResearchInfo = evaluatedApplication.NonFundedResearchInfo,
                EthicsApplicationLog = evaluatedApplication.EthicsApplicationLog,
                EthicsEvaluation = evaluatedApplication.EthicsEvaluation,
                InitialReview = evaluatedApplication.InitialReview,
                AppUser = evaluatedApplication.AppUser // Assuming `User` represents the application user
            };

            return View(evaluationDetailsViewModel);
        }

    }
}
