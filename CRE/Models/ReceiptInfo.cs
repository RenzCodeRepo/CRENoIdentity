using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class ReceiptInfo
    {
        [Key]
        [Required(ErrorMessage ="Please input the Receipt Number.")]
        public string receiptNo { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        [Required(ErrorMessage ="Please input the amound paid.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter the paid amount.")]
        public float amountPaid { get; set; }
        [Required(ErrorMessage ="Select the date you paid the fee.")]
        [DataType(DataType.Date)]
        public DateOnly datePaid { get; set; }
        [Required(ErrorMessage ="Please upload the scanned pdf receipt")]
        public byte[] scanReceipt { get; set; }

        //navigation properties
        public EthicsApplication EthicsApplication { get; set; }
    }
}
