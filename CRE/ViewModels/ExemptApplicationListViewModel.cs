using CRE.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CRE.ViewModels
{
    public class ExemptApplicationListViewModel
    {
        public List<ChiefEvaluationViewModel> ExemptApplications { get; set; }
        public List<EvaluatedExemptApplication> EvaluatedExemptApplications { get; set; }

        public List<EthicsApplication> EvaluatedExpeditedApplications { get; set; } = new List<EthicsApplication>();
        public List<EthicsApplication> EvaluatedFullReviewApplications { get; set; } = new List<EthicsApplication>();
        public ExemptApplicationListViewModel()
        {
            ExemptApplications = new List<ChiefEvaluationViewModel>();
            EvaluatedExemptApplications = new List<EvaluatedExemptApplication>();
        }
    }
}
