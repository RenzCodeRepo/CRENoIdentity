using CRE.Models;
using System.ComponentModel.DataAnnotations;

namespace CRE.ViewModels
{
    public class ApplyEthicsViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }  //start of application
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }//general information
        public User User { get; set; }  //needed dependes on the user type i.e. external or internal
        public ReceiptInfo ReceiptInfo { get; set; } = null; //will be initialized once the user is determined as external
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; } = new List<EthicsApplicationLog>();
        public List<CoProponent> CoProponent { get; set; } = new List<CoProponent>();//multiple researchers


        //other properties needed
        public IFormFile receiptFile { get; set; } //placeholder for conversion
    }
}
