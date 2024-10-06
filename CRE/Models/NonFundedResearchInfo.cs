using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class NonFundedResearchInfo
    {
        // Primary Key
        [Key, MaxLength(30)]
        public string nonFundedResearchId { get; set; }

        // Foreign Keys
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }

        [ForeignKey(nameof(EthicsClearance))]
        public int? ethicsClearanceId { get; set; }

        [ForeignKey(nameof(CompletionCertificate))]
        public int? completionCertId { get; set; }

        [ForeignKey(nameof(User))]
        public int userId { get; set; }

        // Required Fields
        [Required]
        public string title { get; set; }

        [Required]
        public DateTime dateSubmitted { get; set; }

        // Campus, College, and University attributes
        public string campus { get; set; }
        public string college { get; set; }
        public string university { get; set; }

        public DateOnly? completion_Date { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public EthicsClearance EthicsClearance { get; set; }
        public CompletionCertificate CompletionCertificate { get; set; }
        public ICollection<CoProponent> CoProponent { get; set; } = new List<CoProponent>();
    }
}
