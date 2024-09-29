using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsApplication
    {
        [Key, MaxLength(16)]
        public string urecNo { get; set; }
        [ForeignKey(nameof(User))]
        public int userId { get; set; }
        [Required]
        public DateOnly submissionDate { get; set; }
        public string? dtsNo { get; set; }
        [Required]
        public string fieldOfStudy { get; set; }

        //navigation properties
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public User User { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
        public InitialReview InitialReview { get; set; }
        public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; } = new List<EthicsApplicationLog>();
        public ICollection<EthicsApplicationForms> EthicsApplicationForms { get; set; } = new List<EthicsApplicationForms>();
        public ICollection<EthicsEvaluation> EthicsEvaluation { get; set; } = new List<EthicsEvaluation>();
        public CompletionReport CompletionReport { get; set; }
        public EthicsClearance EthicsClearance { get; set; }
    }
}
