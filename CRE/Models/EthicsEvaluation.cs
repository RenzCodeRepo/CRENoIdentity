using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsEvaluation
    {
        [Key]
        public int evaluationId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        [ForeignKey(nameof(EthicsEvaluator))]
        public int ethicsEvaluatorId { get; set; }
        [Required]
        public DateOnly startDate { get; set; }
        public DateOnly? endDate { get; set; }
        [Required]
        public string recommendation { get; set; }
        [Required]
        public string remarks { get; set; }

        //generated pdf answersheets
        public byte[]? protocolReviewSheet { get; set; }

        public byte[]? informedConsentForm { get; set; }



        //navigation properties
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
    }
}
