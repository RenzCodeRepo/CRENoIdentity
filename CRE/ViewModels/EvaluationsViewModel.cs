using CRE.Models;

namespace CRE.ViewModels
{
    public class EvaluationsViewModel
    {
        public IEnumerable<EthicsApplication> ExemptApplications { get; set; }
        public IEnumerable<EthicsEvaluation> EvaluatedExemptApplications { get; set; }
    }
}
