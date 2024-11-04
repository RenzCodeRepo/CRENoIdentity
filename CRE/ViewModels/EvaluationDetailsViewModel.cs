using CRE.Models;
using System.ComponentModel.DataAnnotations;

namespace CRE.ViewModels
{
    public class EvaluationDetailsViewModel
    {
        public AppUser? AppUser { get; set; }
        public Secretariat? Secretariat { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public EthicsEvaluator? EthicsEvaluator { get; set; }
        public ICollection<CoProponent>? CoProponent { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public Chairperson? Chairperson { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public Faculty? Faculty { get; set; }
        public InitialReview? InitialReview { get; set; }
        public EthicsEvaluation EthicsEvaluation { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLog>? EthicsApplicationLog { get; set; }

        // Property for a single evaluation
        public EthicsEvaluation? CurrentEvaluation { get; set; } // This represents the current evaluation.

        // Property for display
        public string UrecNo => EthicsApplication?.urecNo;

        // Optional files for upload, if needed
        [Required(ErrorMessage = "Please upload the Protocol Review Sheet.")]
        public IFormFile? ProtocolReviewSheet { get; set; }

        [Required(ErrorMessage = "Please upload the Informed Consent Form.")]
        public IFormFile? InformedConsentForm { get; set; }

    }
}
