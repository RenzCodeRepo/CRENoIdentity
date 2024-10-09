using CRE.Models;

namespace CRE.ViewModels
{
    public class ApplicationRequirementsViewModel
    {
        public User User { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; } = new List<EthicsApplicationForms>();
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; } = new List<EthicsApplicationLog>();
        public List<CoProponent> CoProponent { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
    }
}
