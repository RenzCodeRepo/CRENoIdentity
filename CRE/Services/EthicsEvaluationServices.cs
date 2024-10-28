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
        public async Task<EthicsEvaluation> GetEvaluationByUrecNoAndIdAsync(string urecNo, int evaluationId)
        {
            return await _context.EthicsEvaluation
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.evaluationId == evaluationId);
        }
        public async Task<List<EvaluatedExemptApplication>> GetEvaluatedExemptApplicationsAsync()
        {
            var evaluatedApplications = await _context.EthicsApplicationLog
                .Where(log => log.status == "Evaluated")
                .Select(log => new EvaluatedExemptApplication
                {
                    EthicsApplication = log.EthicsApplication, // Assuming there's a navigation property
                    NonFundedResearchInfo = log.EthicsApplication.NonFundedResearchInfo, // Assuming navigation property exists
                    EthicsApplicationLog = _context.EthicsApplicationLog
                .Where(l => l.logId == log.logId) // Collecting all logs for the application
                .ToList() // Converting to a list to satisfy IEnumerable requirement
                })
                .Include(e => e.EthicsApplication) // Include navigation properties if necessary
                .Include(e => e.NonFundedResearchInfo)
                .ToListAsync();

            return evaluatedApplications;
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

