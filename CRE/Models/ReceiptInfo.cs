using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class ReceiptInfo
    {
        [Key]
        public string receipt_No { get; set; }
        [ForeignKey(nameof(E_Application))]
        public string urec_No { get; set; }
        [Required]
        public float amount_Paid { get; set; }
        [Required]
        public DateOnly date_Paid { get; set; }
        [Required]
        public byte[] scan_Receipt { get; set; }

        //navigation properties
        public EthicsApplication E_Application { get; set; }
    }
}
