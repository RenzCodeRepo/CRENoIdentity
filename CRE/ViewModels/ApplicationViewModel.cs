using CRE.Models; // Ensure you have the right using directives

namespace CRE.ViewModels
{
    public class ApplicationViewModel
    {
        public string UrecNo { get; set; }
        public string Title { get; set; }
        public AppUser AppUser { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }

    }
}
