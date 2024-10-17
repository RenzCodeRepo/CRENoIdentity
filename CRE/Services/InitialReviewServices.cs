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
                .Where(log => log.status == "Forms Uploaded")
                .GroupBy(log => log.urecNo) // Group by UrecNo to get the latest log per application
                .Select(g => g.OrderByDescending(log => log.changeDate).FirstOrDefault()) // Get the latest log in each group
                .ToListAsync();

            // Step 2: Retrieve all EthicsApplications and their forms for the found logs
            var ethicsApplications = new List<InitialReviewViewModel>();

            foreach (var log in latestLogs)
            {
                var ethicsApplication = await _context.EthicsApplication
                    .FirstOrDefaultAsync(e => e.urecNo == log.urecNo);

                var ethicsApplicationForms = await _context.EthicsApplicationForms
                    .Where(form => form.urecNo == log.urecNo)
                    .ToListAsync();

                ethicsApplications.Add(new InitialReviewViewModel
                {
                    EthicsApplication = ethicsApplication,
                    EthicsApplicationForms = ethicsApplicationForms,
                    EthicsApplicationLog = new List<EthicsApplicationLog> { log } // Add the log if needed
                });
            }

            return ethicsApplications;
        }
    }
}
