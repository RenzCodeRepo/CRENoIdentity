using CRE.Models;
using System.ComponentModel.DataAnnotations;

namespace CRE.ViewModels
{
    public class EvaluationDetailsViewModel
    {
        public AppUser? AppUser { get; set; }
        public Secretariat? Secretariat { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public ICollection<CoProponent>? CoProponent { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public Chairperson? Chairperson { get; set; }
        public EthicsEvaluator? EthicsEvaluator { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public InitialReview? InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLog>? EthicsApplicationLog { get; set; }
        public EthicsEvaluation? EthicsEvaluation { get; set; } = new EthicsEvaluation();


        public string ReviewerName => $"{EthicsEvaluation?.EthicsEvaluator?.Faculty?.User?.fName} {EthicsEvaluation?.EthicsEvaluator?.Faculty?.User?.mName} {EthicsEvaluation?.EthicsEvaluator?.Faculty?.User?.lName}".Trim();
        public string UrecNo => EthicsApplication?.urecNo; // Property for display

        // Optional files for upload, if needed
        [Required(ErrorMessage = "Please upload the Protocol Review Sheet.")]
        public IFormFile? ProtocolReviewSheet { get; set; }

        [Required(ErrorMessage = "Please upload the Informed Consent Form.")]
        public IFormFile? InformedConsentForm { get; set; }
    }
}
