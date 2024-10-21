using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class InitialReviewServices : IInitialReviewServices
    {
        private readonly ApplicationDbContext _context;
        public InitialReviewServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<InitialReviewViewModel>> GetEthicsApplicationsForInitialReviewAsync()
        {
            // Step 1: Get all the latest logs with "Forms Uploaded"
            var latestLogs = await _context.EthicsApplicationLog
                .Where(log => log.status == "Pending for Evaluation")
                .GroupBy(log => log.urecNo) // Group by UrecNo to get the latest log per application
                .Select(g => g.OrderByDescending(log => log.changeDate).FirstOrDefault()) // Get the latest log in each group
                .ToListAsync();

            // Step 2: Retrieve all EthicsApplications and their forms for the found logs
            var ethicsApplications = new List<InitialReviewViewModel>();

            foreach (var log in latestLogs)
            {
                var ethicsApplication = await _context.EthicsApplication
                    .FirstOrDefaultAsync(e => e.urecNo == log.urecNo);
                // Ensure you also retrieve NonFundedResearchInfo if it's linked
                var nonFundedResearchInfo = await _context.NonFundedResearchInfo
                    .FirstOrDefaultAsync(nf => nf.urecNo == log.urecNo); // Use appropriate link

                var ethicsApplicationForms = await _context.EthicsApplicationForms
                    .Where(form => form.urecNo == log.urecNo)
                    .ToListAsync();

                ethicsApplications.Add(new InitialReviewViewModel
                {
                    EthicsApplication = ethicsApplication,
                    EthicsApplicationForms = ethicsApplicationForms,
                    NonFundedResearchInfo = nonFundedResearchInfo,
                    EthicsApplicationLog = new List<EthicsApplicationLog> { log } // Add the log if needed
                });
            }

            return ethicsApplications;
        }
        // New method for getting application details
        public async Task<InitialReviewViewModel> GetApplicationDetailsAsync(string urecNo)
        {
            var application = await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)  // Load NonFundedResearchInfo
                    .ThenInclude(nf => nf.CoProponent)  // Load CoProponents from NonFundedResearchInfo
                .Include(e => e.ReceiptInfo)
                .Include(e => e.EthicsApplicationLog)
                .Include(e => e.EthicsApplicationForms)
                .FirstOrDefaultAsync(e => e.urecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found.");
            }
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == application.userId);
            var viewModel = new InitialReviewViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo.CoProponent, // Now fetching CoProponents from NonFundedResearchInfo
                ReceiptInfo = application.ReceiptInfo,
                EthicsApplicationLog = application.EthicsApplicationLog.OrderByDescending(log => log.changeDate),
                EthicsApplicationForms = application.EthicsApplicationForms,
                InitialReview = await _context.InitialReview.FirstOrDefaultAsync(ir => ir.urecNo == urecNo),
                AppUser = appUser
            };

            return viewModel;
        }



        public async Task<IEnumerable<CoProponent>> GetCoProponentsByNonFundedResearchIdAsync(string nonFundedResearchId)
        {
            return await _context.CoProponent
                .Where(cp => cp.nonFundedResearchId == nonFundedResearchId)
                .ToListAsync();
        }

        public async Task ApproveApplicationAsync(string urecNo, string comments, string userId)
        {
            // Find the existing initial review
            var initialReview = await _context.InitialReview
                .FirstOrDefaultAsync(ir => ir.urecNo == urecNo);

            if (initialReview == null)
            {
                // If no existing review, create a new one
                initialReview = new InitialReview
                {
                    urecNo = urecNo,
                    status = "Approved",
                    userId = userId,
                    feedback = comments,
                    dateReviewed = DateOnly.FromDateTime(DateTime.Now)
                };

                // Add the new initial review to the context
                await _context.InitialReview.AddAsync(initialReview);
            }
            else
            {
                // Update the existing initial review
                initialReview.status = "Approved";
                initialReview.userId = userId;
                initialReview.feedback = comments;
                initialReview.dateReviewed = DateOnly.FromDateTime(DateTime.Now);

                // Update the record
                _context.InitialReview.Update(initialReview);
            }

            // Create a new log entry for the Ethics Application
            var logEntry = new EthicsApplicationLog
            {
                urecNo = urecNo,
                status = "Approved for Evaluation",
                userId = userId,
                changeDate = DateTime.Now,
                comments = comments
            };

            // Add the new log entry to the context
            await _context.EthicsApplicationLog.AddAsync(logEntry);

            // Save changes to both the initial review and the log
            await _context.SaveChangesAsync();
        }

        public async Task ReturnApplicationAsync(string urecNo, string comments, string userId)
        {
            // Find the existing initial review
            var initialReview = await _context.InitialReview
                .FirstOrDefaultAsync(ir => ir.urecNo == urecNo);

            if (initialReview == null)
            {
                // If no existing review, create a new one
                initialReview = new InitialReview
                {
                    urecNo = urecNo,
                    status = "Returned",
                    feedback = comments,
                    userId = userId,
                    dateReviewed = DateOnly.FromDateTime(DateTime.Now)
                };

                // Add the new initial review to the context
                await _context.InitialReview.AddAsync(initialReview);
            }
            else
            {
                // Update the existing initial review
                initialReview.status = "Not Approved";
                initialReview.feedback = comments;
                initialReview.userId = userId; 
                initialReview.dateReviewed = DateOnly.FromDateTime(DateTime.Now);

                // Update the record
                _context.InitialReview.Update(initialReview);
            }

            // Create a new log entry for the Ethics Application
            var logEntry = new EthicsApplicationLog
            {
                urecNo = urecNo,
                status = "Returned for Revisions", // Update the status to reflect the return
                userId = userId,
                changeDate = DateTime.Now,
                comments = comments
            };

            // Add the new log entry to the context
            await _context.EthicsApplicationLog.AddAsync(logEntry);

            // Save changes to both the initial review and the log
            await _context.SaveChangesAsync();
        }
        // Fetch pending applications for initial review
        public async Task<IEnumerable<InitialReviewViewModel>> GetPendingApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                .Include(e => e.EthicsApplicationLog)
                .Where(e => e.EthicsApplicationLog
                    .OrderByDescending(log => log.changeDate)  // Get logs in descending order by changeDate
                    .FirstOrDefault().status == "Pending for Evaluation") // Check if the latest log's status is "Pending"
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e,
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    // Fetch only the latest log with status "Pending for Evaluation"
                    EthicsApplicationLog = e.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .Take(1) // Take only the latest log entry
                })
                .ToListAsync();
        }

        // Fetch approved applications for initial review
        public async Task<IEnumerable<InitialReviewViewModel>> GetApprovedApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                .Include(e => e.EthicsApplicationLog)
                .Where(e => e.EthicsApplicationLog
                    .OrderByDescending(log => log.changeDate)  // Get logs in descending order by changeDate
                    .FirstOrDefault().status == "Approved for Evaluation") // Check if the latest log's status is "Approved"
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e,
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    // Fetch only the latest log with status "Approved for Evaluation"
                    EthicsApplicationLog = e.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .Take(1) // Take only the latest log entry
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InitialReviewViewModel>> GetReturnedApplicationsAsync()
        {
            // Fetch applications that have been returned
            return await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                .Include(e => e.EthicsApplicationLog)
                .Where(e => e.EthicsApplicationLog
                    .OrderByDescending(log => log.changeDate) // Order logs by changeDate
                    .FirstOrDefault().status == "Returned for Revisions") // Check if the latest log's status is "Returned"
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e,
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    // Fetch only the latest log with status "Returned"
                    EthicsApplicationLog = e.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .Take(1) // Take only the latest log entry
                })
                .ToListAsync();
        }

        public async Task<InitialReview> GetInitialReviewByUrecNoAsync(string urecNo)
        {
            return await _context.InitialReview
            .Include(ir => ir.EthicsApplication)  // Include related data if needed
            .FirstOrDefaultAsync(ir => ir.EthicsApplication.urecNo == urecNo);
        }
    }

}
