using CRE.Models;

namespace CRE.ViewModels
{
    public class ChairpersonApplicationsViewModel
    {
        public IEnumerable<EthicsApplication> UnassignedApplications { get; set; }
        public IEnumerable<EthicsApplication> UnderEvaluationApplications { get; set; }
        public IEnumerable<EthicsApplication> EvaluationResultApplications { get; set; }
        public Dictionary<string, List<string>> ApplicationEvaluatorNames { get; set; } = new();
        public IEnumerable<NonFundedResearchInfo> NonFundedResearchInfo { get; set; }
    }

}
