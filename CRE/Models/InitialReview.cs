using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class InitialReview
    {
        [Key]
        public int initalReviewId { get; set; }
        [ForeignKey(nameof(Secretariat))]
        public int secretariatId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        public DateOnly? dateReviewed { get; set; }
        [Required]
        public string status { get; set; }
        public string feedback { get; set; }

        //navigation properties
        public Secretariat Secretariat { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
    }
}
