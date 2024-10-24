using CRE.Models;

namespace CRE.ViewModels
{
    public class ChairpersonApplicationsViewModel
    {
        public IEnumerable<EthicsApplication> UnassignedApplications { get; set; }
        public IEnumerable<EthicsApplication> UnderEvaluationApplications { get; set; }
        public IEnumerable<EthicsApplication> EvaluationResultApplications { get; set; }
    }

}
