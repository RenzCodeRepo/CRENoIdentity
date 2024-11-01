using CRE.Models;

namespace CRE.ViewModels
{
    public class EvaluatedExpeditedApplication
    {
        public EthicsApplication EthicsApplication { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public ICollection<EthicsEvaluation> EthicsEvaluation { get; set; }
        public InitialReview InitialReview { get; set; }
        public AppUser User { get; set; }
        public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; }
    }
}
