using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace CRE.Services
{
    public class EthicsEvaluationServices : IEthicsEvaluationServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsEvaluationServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateEvaluation(EthicsEvaluation ethicsEvaluation)
        {
            _context.EthicsEvaluation.Add(ethicsEvaluation);
            _context.SaveChanges();
        }

        public EthicsEvaluation GetEvaluationByUrecNo(string urecNo)
        {
            return _context.EthicsEvaluation
          .FirstOrDefault(e => e.urecNo == urecNo);
        }
        public async Task UpdateEvaluationStatusAsync(int evaluationId, string status)
        {
            var evaluation = await _context.EthicsEvaluation.FindAsync(evaluationId);
            if (evaluation != null)
            {
                evaluation.evaluationStatus = status; // "Accepted" or "Declined"
                await _context.SaveChangesAsync();
            }
        }
        public async Task AssignEvaluatorAsync(string urecNo, int evaluatorId)
        {
            var ethicsEvaluation = new EthicsEvaluation
            {
                urecNo = urecNo,
                ethicsEvaluatorId = evaluatorId,
                startDate = DateOnly.FromDateTime(DateTime.Now),
                recommendation = "Pending",
                remarks = string.Empty,
                evaluationStatus = "Assigned"
            };

            _context.EthicsEvaluation.AddAsync(ethicsEvaluation);
            await _context.SaveChangesAsync();
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

        public async Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo)
        {
            var application = await _context.EthicsApplication
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

    }
}
