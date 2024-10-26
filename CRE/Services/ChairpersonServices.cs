using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class ChairpersonServices : IChairpersonServices
    {
        private readonly ApplicationDbContext _context;

        public ChairpersonServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EthicsApplication>> GetApplicationsByFieldOfStudyAsync(string userId)
        {
            // Get the chairperson's faculty using the user ID
            var chairperson = await _context.Chairperson
                .Include(c => c.Faculty)
                .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(c => c.Faculty.userId == userId);

            if (chairperson == null)
                return new List<EthicsApplication>(); // Return an empty list if chairperson not found

            // Retrieve applications matching the chairperson's field of study and specific review types, including evaluations and evaluators
            var applications = await _context.EthicsApplication
                .Include(e => e.InitialReview)
                .Include(e => e.EthicsEvaluation) // Include EthicsEvaluation entities
                    .ThenInclude(e => e.EthicsEvaluator) // Include the related EthicsEvaluator entities
                .Where(e => e.fieldOfStudy == chairperson.fieldOfStudy
                             && (e.InitialReview.ReviewType == "Full Review"
                             || e.InitialReview.ReviewType == "Expedited"))
                .ToListAsync();

            return applications;
        }


        public async Task AssignEvaluatorsAsync(string urecNo, List<int> evaluatorIds)
        {
            foreach (var evaluatorId in evaluatorIds)
            {
                var evaluation = new EthicsEvaluation
                {
                    urecNo = urecNo,
                    ethicsEvaluatorId = evaluatorId,
                    startDate = DateOnly.FromDateTime(DateTime.Now),
                    recommendation = string.Empty, // Initialize empty, evaluators will fill this
                    remarks = string.Empty, // Initialize empty
                };

                _context.EthicsEvaluation.Add(evaluation);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<EthicsApplication> GetApplicationAsync(string urecNo)
        {
            return await _context.EthicsApplication
                .Include(e => e.InitialReview) // Include InitialReview for ReviewType
                .FirstOrDefaultAsync(e => e.urecNo == urecNo);
        }
    }
}
