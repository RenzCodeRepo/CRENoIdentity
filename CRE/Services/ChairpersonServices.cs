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
                .FirstOrDefaultAsync(c => c.Faculty.userId == userId); // Adjusted to match your userId

            if (chairperson == null)
                return new List<EthicsApplication>(); // Return an empty list if chairperson not found

            // Retrieve applications matching the chairperson's field of study and specific review types, including evaluations and evaluators
            var applications = await _context.EthicsApplication
                .Include(e => e.InitialReview) // Include InitialReview
                .Include(e => e.EthicsEvaluation) // Include EthicsEvaluation entities
                    .ThenInclude(e => e.EthicsEvaluator) // Include the related EthicsEvaluator entities
                        .ThenInclude(ev => ev.Faculty) // Include Faculty for each EthicsEvaluator
                            .ThenInclude(f => f.User) // Include User for each Faculty
                .Where(e => e.fieldOfStudy == chairperson.fieldOfStudy // Ensure correct casing for FieldOfStudy
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

                    // Initialize fields for protocol evaluation
                    ProtocolRecommendation = "Pending", // Default status
                    ProtocolRemarks = string.Empty,

                    // Initialize fields for consent evaluation
                    ConsentRecommendation = "Pending", // Default status
                    ConsentRemarks = string.Empty,

                    // Set initial status
                    evaluationStatus = "Assigned" // Mark as assigned since evaluators are assigned
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
