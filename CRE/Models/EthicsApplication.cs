using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsApplication
    {
        [Key, MaxLength(30)]
        [Display(Name = "UREC NO.")]
        public string urecNo { get; set; }
        [ForeignKey(nameof(User))]
        public string? userId { get; set; }
        [Required]
        public DateOnly submissionDate { get; set; }
        [RegularExpression(@"\d{4}-\d{4}-\d{2}", ErrorMessage = "DTS No. must be in the format xxxx-xxxx-xx.")]
        public string? dtsNo { get; set; }
        [Required(ErrorMessage ="Field of Study is Required.")]
        [Display(Name ="Field of Study: ")]
        public string fieldOfStudy { get; set; }

        //navigation properties
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public AppUser? User { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public InitialReview? InitialReview { get; set; }
        public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; } = new List<EthicsApplicationLog>();
        public ICollection<EthicsApplicationForms> EthicsApplicationForms { get; set; } = new List<EthicsApplicationForms>();
        public ICollection<EthicsEvaluation>? EthicsEvaluation { get; set; } = new List<EthicsEvaluation>();
        public CompletionReport? CompletionReport { get; set; }
        public EthicsClearance? EthicsClearance { get; set; }
    }
}
