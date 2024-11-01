using CRE.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CRE.ViewModels
{
    public class ApplicationEvaluationViewModel
    {
        public List<ChiefEvaluationViewModel> ExemptApplications { get; set; }
        public List<EvaluatedExemptApplication> EvaluatedExemptApplications { get; set; }
        public List<EvaluatedExpeditedApplication> EvaluatedExpeditedApplications { get; set; }
        public List<EvaluatedFullReviewApplication> EvaluatedFullReviewApplications { get; set; } 
        public ApplicationEvaluationViewModel()
        {
            ExemptApplications = new List<ChiefEvaluationViewModel>();
            EvaluatedExemptApplications = new List<EvaluatedExemptApplication>();
            EvaluatedExpeditedApplications = new List<EvaluatedExpeditedApplication>();
            EvaluatedFullReviewApplications = new List<EvaluatedFullReviewApplication>();
        }
    }
}
