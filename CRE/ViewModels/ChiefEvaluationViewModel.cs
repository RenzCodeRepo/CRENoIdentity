using CRE.Models;
using System.ComponentModel.DataAnnotations;

namespace CRE.ViewModels
{
    public class ChiefEvaluationViewModel
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
        // Example properties for displaying chief and evaluator names
        public string ChiefName => $"{EthicsEvaluation?.Chief?.User?.fName} {EthicsEvaluation?.Chief?.User?.mName} {EthicsEvaluation?.Chief?.User?.lName}".Trim();

        // Ethics Evaluator Full Name
        public string EvaluatorName => $"{EthicsEvaluation?.EthicsEvaluator?.Faculty?.User?.fName} {EthicsEvaluation?.EthicsEvaluator?.Faculty?.User?.mName} {EthicsEvaluation?.EthicsEvaluator?.Faculty?.User?.lName}".Trim();

        public string urecNo => EthicsApplication?.urecNo;

        [Required(ErrorMessage = "Please upload the Protocol Review Sheet.")]
        public IFormFile ProtocolReviewSheet { get; set; }

        [Required(ErrorMessage = "Please upload the Informed Consent Form.")]
        public IFormFile InformedConsentForm { get; set; }

    }
}
