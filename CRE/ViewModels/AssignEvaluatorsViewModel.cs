using CRE.Models; // Assuming this is where your models are located
using System.Collections.Generic;

namespace CRE.ViewModels
{
    public class AssignEvaluatorsViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public List<EthicsEvaluator> AvailableEvaluators { get; set; } = new List<EthicsEvaluator>();
        public List<int> SelectedEvaluatorIds { get; set; } = new List<int>();
        
    }
}
