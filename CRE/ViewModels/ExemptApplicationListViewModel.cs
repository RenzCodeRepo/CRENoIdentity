namespace CRE.ViewModels
{
    public class ExemptApplicationListViewModel
    {
        public List<ChiefEvaluationViewModel> ExemptApplications { get; set; }
        public List<EvaluatedExemptApplication> EvaluatedExemptApplications { get; set; }

        public ExemptApplicationListViewModel()
        {
            ExemptApplications = new List<ChiefEvaluationViewModel>();
            EvaluatedExemptApplications = new List<EvaluatedExemptApplication>();
        }
    }
}
