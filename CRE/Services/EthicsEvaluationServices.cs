using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System;

namespace CRE.Services
{
    public class EthicsEvaluationServices : IEthicsEvaluationServices
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EthicsEvaluationServices(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CreateEvaluationAsync(EthicsEvaluation evaluation)
        {
            // Add the new evaluation to the context
            await _context.EthicsEvaluation.AddAsync(evaluation);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the created evaluation ID
            return evaluation.evaluationId; // Assuming EvaluationId is auto-generated
        }
        public async Task<List<EthicsEvaluator>> GetAllEvaluatorsAsync()
        {
            return await _context.EthicsEvaluator
                .Include(e => e.Faculty)                      // Include Faculty data
                    .ThenInclude(f => f.User)                 // Include User data within Faculty
                .Include(e => e.EthicsEvaluatorExpertise)     // Include evaluator expertise collection
                    .ThenInclude(exp => exp.Expertise)        // Include each Expertise within EthicsEvaluatorExpertise
                .ToListAsync();
        }
        public EthicsEvaluation GetEvaluationByUrecNo(string urecNo)
        {
            return _context.EthicsEvaluation
          .FirstOrDefault(e => e.urecNo == urecNo);
        }
        public async Task<EthicsEvaluation> GetEvaluationByUrecNoAndIdAsync(string urecNo, int evaluationId)
        {
            return await _context.EthicsEvaluation
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.evaluationId == evaluationId);
        }
        public async Task UpdateEvaluationAsync(EthicsEvaluation ethicsEvaluation)
        {
            _context.EthicsEvaluation.Update(ethicsEvaluation); // Marks the entity as modified
            await _context.SaveChangesAsync();
        }
        public async Task<EthicsEvaluation> GetEvaluationByUrecNoAndEvaluatorIdAsync(string urecNo, int ethicsEvaluatorId)
        {
            return await _context.EthicsEvaluation
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.ethicsEvaluatorId == ethicsEvaluatorId);
        }

        public async Task<EvaluationDetailsViewModel> GetEvaluationDetailsAsync(int evaluationId)
        {
            // Fetch the evaluation along with related entities
            var evaluation = await _context.EthicsEvaluation
                .Include(ev => ev.EthicsEvaluator)
                    .ThenInclude(e => e.Faculty)
                        .ThenInclude(f => f.User)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.NonFundedResearchInfo)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.ReceiptInfo)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationForms) // Include forms
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationLog) // Include logs
                .FirstOrDefaultAsync(ev => ev.evaluationId == evaluationId); // Use EvaluationId as int

            if (evaluation == null)
            {
                throw new Exception("Evaluation not found.");
            }

            // Retrieve the associated application for further details
            var application = await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Make sure we include related info
                    .ThenInclude(nf => nf.CoProponent) // Assuming you want to get co-proponents too
                .Include(a => a.ReceiptInfo)
                .FirstOrDefaultAsync(a => a.urecNo == evaluation.urecNo); // Use the appropriate ID

            // If application is not found, handle it accordingly
            if (application == null)
            {
                throw new Exception("Associated application not found.");
            }

            // Create and populate the ViewModel
            var viewModel = new EvaluationDetailsViewModel
            {
                CurrentEvaluation = evaluation, // Use the current evaluation
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo, // No null propagation
                ReceiptInfo = application.ReceiptInfo,
                AppUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == application.userId), // Handle null before
                EthicsEvaluator = evaluation.EthicsEvaluator, // Initialize the EthicsEvaluator
                EthicsApplicationForms = application.EthicsApplicationForms, // Initialize forms
                EthicsApplicationLog = application.EthicsApplicationLog // Initialize logs
            };

            return viewModel;
        }

        public async Task<EvaluatedExemptApplication> GetEvaluationDetailsAsync(string urecNo, int evaluationId)
        {
            // Fetching the evaluation based on both urecNo and evaluationId
            var evaluation = await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication) // Include the EthicsApplication
                    .ThenInclude(a => a.NonFundedResearchInfo) // Include NonFundedResearchInfo
                .Include(e => e.EthicsApplication.EthicsApplicationLog) // Include EthicsApplicationLog
                .Include(e => e.EthicsApplication.InitialReview) // Include InitialReview from EthicsApplication
                .Include(e => e.Chief) // Include Chief
                    .ThenInclude(c => c.User) // Include the associated User of the Chief
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.evaluationId == evaluationId);

            if (evaluation == null)
                return null;

            return new EvaluatedExemptApplication
            {
                EthicsApplication = evaluation.EthicsApplication,
                NonFundedResearchInfo = evaluation.EthicsApplication?.NonFundedResearchInfo,
                EthicsApplicationLog = evaluation.EthicsApplication?.EthicsApplicationLog,
                EthicsEvaluation = evaluation,
                InitialReview = evaluation.EthicsApplication.InitialReview, // Set the InitialReview property from EthicsApplication
                User = evaluation.Chief?.User // Set the User associated with the Chief
            };
        }
        public async Task<EvaluationDetailsViewModel> GetEvaluationAndEvaluatorDetailsAsync(string urecNo, int evaluationId)
        {
            var evaluation = await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsApplication.EthicsApplicationLog)
                .Include(e => e.EthicsApplication.InitialReview)
                .Include(e => e.EthicsEvaluator)
                    .ThenInclude(evaluator => evaluator.Faculty.User)
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.evaluationId == evaluationId);

            if (evaluation == null)
                return null;

            var evaluatorUser = evaluation.EthicsEvaluator?.Faculty?.User;

            return new EvaluationDetailsViewModel
            {
                EthicsApplication = evaluation.EthicsApplication,
                NonFundedResearchInfo = evaluation.EthicsApplication?.NonFundedResearchInfo,
                EthicsApplicationLog = evaluation.EthicsApplication?.EthicsApplicationLog,
                EthicsEvaluation = evaluation,
                InitialReview = evaluation.EthicsApplication.InitialReview,
                AppUser = evaluatorUser // List of user details for evaluators
            };
        }

        public async Task UpdateEvaluationStatusAsync(int evaluationId, string status, string? reasonForDecline, int ethicsEvaluatorId)
        {
            var evaluation = await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication) // Eager load EthicsApplication to access UREC number
                .FirstOrDefaultAsync(e => e.evaluationId == evaluationId); // Using FirstOrDefault for clarity

            if (evaluation != null)
            {
                if (status == "Declined")
                {
                    // Create a new DeclinedEvaluation entry
                    var declinedEvaluation = new EthicsEvaluationDeclined
                    {
                        evaluationId = evaluationId,
                        reasonForDecline = reasonForDecline,
                        urecNo = evaluation.EthicsApplication?.urecNo, // Access UREC number here
                        ethicsEvaluatorId = ethicsEvaluatorId,
                        declineDate = DateOnly.FromDateTime(DateTime.UtcNow) // Set decline date to today's date
                    };

                    // Add the declined evaluation to the context
                    await _context.EthicsEvaluationDeclined.AddAsync(declinedEvaluation);

                    // Remove the evaluation from EthicsEvaluation
                    _context.EthicsEvaluation.Remove(evaluation);
                }
                else
                {
                    evaluation.evaluationStatus = status; // Update status for other cases
                    evaluation.reasonForDecline = null; // Clear the reason for accepted evaluations
                    evaluation.startDate = DateOnly.FromDateTime(DateTime.UtcNow);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignEvaluatorAsync(string urecNo, int evaluatorId)
        {
            var ethicsEvaluation = new EthicsEvaluation
            {
                urecNo = urecNo,
                ethicsEvaluatorId = evaluatorId,

                // Initialize protocol and consent recommendations to pending
                ProtocolRecommendation = "Pending",
                ProtocolRemarks = string.Empty,

                ConsentRecommendation = "Pending",
                ConsentRemarks = string.Empty,

                // Set initial status
                evaluationStatus = "Assigned"
            };

            await _context.EthicsEvaluation.AddAsync(ethicsEvaluation);
            await _context.SaveChangesAsync();
        }

        public async Task IncrementDeclinedAssignmentCountAsync(int ethicsEvaluatorId)
        {
            var evaluator = await _context.EthicsEvaluator.FindAsync(ethicsEvaluatorId);
            if (evaluator != null)
            {
                evaluator.declinedAssignment += 1;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<EthicsEvaluator>> GetAvailableEvaluatorsAsync(string fieldOfStudy)
        {
            return await _context.EthicsEvaluator
                .Include(e => e.Faculty)
                .ThenInclude(f => f.User)  // Include User
                .Include(e => e.EthicsEvaluatorExpertise)
                .ThenInclude(ee => ee.Expertise)
                .Where(e => e.Faculty != null && e.Faculty.User != null)  // Check for non-null Faculty and User
                .Where(e => e.EthicsEvaluatorExpertise.Any(ee => ee.Expertise.expertiseName == fieldOfStudy))  // Match expertise
                .ToListAsync();

        }
        public async Task SaveEvaluationAsync(EthicsEvaluation ethicsEvaluation)
        {
            // First, find the existing EthicsApplication by urecNo
            await _context.EthicsEvaluation.AddAsync(ethicsEvaluation);
            await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetEvaluatedUrecNosAsync()
        {
            return await _context.EthicsEvaluation
                .Select(e => e.urecNo) // Assuming `urecNo` is the identifier
                .ToListAsync();
        }
        public async Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo)
        {
            var application = await _context.EthicsApplication
                .Include(e => e.InitialReview)
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent)
                .Include(e => e.EthicsApplicationLog)
                .Include(e => e.ReceiptInfo)
                .Include(e => e.EthicsApplicationForms)
                .FirstOrDefaultAsync(e => e.urecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found.");
            }

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == application.userId);

            var viewModel = new AssignEvaluatorsViewModel
            {
                EthicsApplication = application,
                User = appUser, // Set User here
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo?.CoProponent,
                EthicsApplicationForms = application.EthicsApplicationForms,
                ReceiptInfo = application.ReceiptInfo
            };

            return viewModel;
        }
        // Assuming EthicsApplication has a navigation property for InitialReview
        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetAssignedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.ethicsEvaluatorId == evaluatorId
                            && e.evaluationStatus == "Assigned"
                            && e.evaluationStatus != "Accepted") // Exclude "Accepted" evaluations
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator,
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetAcceptedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.ethicsEvaluatorId == evaluatorId && e.evaluationStatus == "Accepted")
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator,
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetCompletedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.ethicsEvaluatorId == evaluatorId && e.evaluationStatus == "Evaluated")
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator,
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetDeclinedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluationDeclined
                .Include(de => de.EthicsEvaluation) // Include the original evaluation details
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.ReceiptInfo) // Include ReceiptInfo if needed
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.EthicsApplicationLog) // Include application logs if needed
                .Where(de => de.EthicsEvaluation.ethicsEvaluatorId == evaluatorId)
                .Select(de => new AssignedEvaluationViewModel
                {
                    EthicsApplication = de.EthicsApplication,
                    EthicsEvaluation = de.EthicsEvaluation, // Original evaluation details
                    EthicsEvaluator = null, // You may need to retrieve evaluator info if available
                    NonFundedResearchInfo = de.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = de.EthicsApplication.InitialReview,
                    EthicsApplicationLogs = de.EthicsApplication.EthicsApplicationLog,
                    ReceiptInfo = de.EthicsApplication.ReceiptInfo,
                    EthicsEvaluationDeclined = de // Set the declined evaluation details
                })
                .ToListAsync();
        }


        public async Task<EthicsEvaluator> GetEvaluatorByUserIdAsync(string userId)
        {
            // Find the Faculty associated with the userId
            var faculty = await _context.Faculty
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.userId == userId);

            // If faculty is not found, return null or throw an exception based on your error handling preference
            if (faculty == null)
            {
                return null; // or throw new Exception("Faculty not found");
            }

            // Find the EthicsEvaluator associated with the facultyId
            return await _context.EthicsEvaluator
                .FirstOrDefaultAsync(e => e.facultyId == faculty.facultyId);
        }

        public async Task<IEnumerable<EthicsEvaluator>> GetAvailableEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string applicantFirstName, string applicantMiddleName, string applicantLastName)
        {
            return allEvaluators
                .Where(e => e.Faculty?.User?.fName != applicantFirstName ||
                             e.Faculty?.User?.mName != applicantMiddleName ||
                             e.Faculty?.User?.lName != applicantLastName) // Exclude applicant by matching names
                .OrderBy(e => e.pendingEval); // Sort by least pending evaluations
        }

        public async Task<IEnumerable<EthicsEvaluator>> GetRecommendedEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string requiredFieldOfStudy, string applicantFirstName, string applicantMiddleName, string applicantLastName)
        {
            return allEvaluators
                .Where(e => e.EthicsEvaluatorExpertise
                    .Any(exp => exp.Expertise != null && exp.Expertise.expertiseName == requiredFieldOfStudy)) // Filter by required expertise
                .Where(e => e.Faculty?.User?.fName != applicantFirstName ||
                             e.Faculty?.User?.mName != applicantMiddleName ||
                             e.Faculty?.User?.lName != applicantLastName) // Exclude applicant by name
                .OrderBy(e => e.pendingEval) // Sort by least pending evaluations
                .Take(3); // Take top 3 recommended evaluators
        }


        public async Task<List<ChiefEvaluationViewModel>> GetExemptApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.AppUser)
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.CoProponent)
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
                }).ToListAsync();
        }

        public async Task<List<EvaluatedExemptApplication>> GetEvaluatedExemptApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.AppUser)
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.CoProponent)
                .Include(a => a.EthicsEvaluation) // Include the EthicsEvaluation to access ChiefId
                .ThenInclude(e => e.Chief) // Assuming Chief is the navigation property in EthicsEvaluation
                .Include(a => a.InitialReview) // Include InitialReview if you need it
                .Where(a => a.InitialReview.ReviewType == "Exempt" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedExemptApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.FirstOrDefault(),
                    InitialReview = a.InitialReview,
                    User = a.User,
                    EthicsApplicationLog = a.EthicsApplicationLog,
                    Chief = a.EthicsEvaluation.FirstOrDefault().Chief // Retrieve the chief based on the evaluation
                })
                .ToListAsync();
        }

        public async Task<List<EvaluatedExpeditedApplication>> GetEvaluatedExpeditedApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.EthicsEvaluation)
                .Where(a => a.InitialReview.ReviewType == "Expedited" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedExpeditedApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.ToList(),
                    InitialReview = a.InitialReview,
                    User = a.User,
                    EthicsApplicationLog = a.EthicsApplicationLog
                }).ToListAsync();
        }

        public async Task<List<EvaluatedFullReviewApplication>> GetEvaluatedFullReviewApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                .Where(a => a.InitialReview.ReviewType == "Full Review" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedFullReviewApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.ToList(),
                    InitialReview = a.InitialReview,
                    User = a.User,
                    EthicsApplicationLog = a.EthicsApplicationLog
                }).ToListAsync();
        }

        public async Task<EvaluationDetailsViewModel> GetEvaluationDetailsWithUrecNoAsync(string urecNo, int evaluationId)
        {
            // Fetch the application based on urecNo and evaluationId
            var application = await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(a => a.CoProponent)
                .Include(a => a.User)

                .Include(a => a.EthicsApplicationForms)
                .Include(a => a.InitialReview)
                .Include(a => a.EthicsEvaluation)
                .FirstOrDefaultAsync(a => a.urecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found");
            }

            var evaluation = await _context.EthicsEvaluation
                .FirstOrDefaultAsync(e => e.evaluationId == evaluationId);

            if (evaluation == null)
            {
                throw new Exception("Evaluation not found");
            }

            // Create and populate the ViewModel
            var model = new EvaluationDetailsViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                AppUser = application.User,
                CoProponent = application.NonFundedResearchInfo.CoProponent.ToList(),
                EthicsApplicationForms = application.EthicsApplicationForms,
                InitialReview = application.InitialReview,
                EthicsEvaluation = evaluation,
                ReceiptInfo = application.ReceiptInfo // Assuming you have a ReceiptInfo property
            };

            return model;
        }

        public async Task<List<EthicsEvaluator>> GetPendingEvaluatorsAsync(string urecNo)
        {
            return await _context.EthicsEvaluator
                .Where(e => e.EthicsEvaluation.Any(a =>
                    a.EthicsApplication.urecNo == urecNo && a.evaluationStatus == "Assigned"))
                .ToListAsync();
        }

        public async Task<List<EthicsEvaluator>> GetAcceptedEvaluatorsAsync(string urecNo)
        {
            return await _context.EthicsEvaluator
                .Where(e => e.EthicsEvaluation.Any(a =>
                    a.EthicsApplication.urecNo == urecNo && a.evaluationStatus == "Accepted"))
                .ToListAsync();
        }
        public async Task<List<EthicsEvaluator>> GetDeclinedEvaluatorsAsync(string urecNo)
        {
            return await _context.EthicsEvaluator
           .Where(e => e.EthicsEvaluation.Any(a =>
               a.EthicsApplication.urecNo == urecNo && a.evaluationStatus == "Declined"))
           .ToListAsync();
        }
    }
}
