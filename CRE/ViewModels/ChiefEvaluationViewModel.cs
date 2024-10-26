using CRE.Extensions;
using CRE.Models;

namespace CRE.ViewModels
{
    public class ChiefEvaluationViewModel
    {
        public AppUser AppUser { get; set; }
        public Secretariat Secretariat { get; set; }
        public Chief Chief { get; set; } // New property to hold Chief information
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public ICollection<CoProponent> CoProponent { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
        public Chairperson Chairperson { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public InitialReview InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public EthicsEvaluation EthicsEvaluation { get; set; }

        // Flattened properties for easy access
        public string ReviewType => InitialReview?.ReviewType;
        public string LatestStatus => EthicsApplicationLog?.OrderByDescending(log => log.changeDate).FirstOrDefault()?.status;

        // Evaluation-specific fields
        public string ChiefComments { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public string EvaluationOutcome { get; set; }

        // Helper property
        public bool HasEthicsEvaluation => EthicsEvaluation != null;

        // Additional properties for your view
        public string ChiefName { get; set; } 
        public int EvaluationId => InitialReview?.initalReviewId ?? 0; // Evaluation ID
    }
}
