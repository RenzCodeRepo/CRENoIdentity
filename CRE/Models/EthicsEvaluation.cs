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
        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
        public string? recommendation { get; set; }
        public string? remarks { get; set; }
        [Required]
        public string evaluationStatus { get; set; } = "Pending";

        //generated pdf answersheets
        public byte[]? protocolReviewSheet { get; set; }
        public byte[]? informedConsentForm { get; set; }



        //navigation properties
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
    }
}
