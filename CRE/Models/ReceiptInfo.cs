using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class ReceiptInfo
    {
        [Key]
        public string receiptNo { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        [Required]
        public float amountPaid { get; set; }
        [Required]
        public DateOnly datePaid { get; set; }
        [Required]
        public byte[] scanReceipt { get; set; }

        //navigation properties
        public EthicsApplication EthicsApplication { get; set; }
    }
}
