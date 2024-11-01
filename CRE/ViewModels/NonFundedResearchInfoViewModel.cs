using CRE.Models;

namespace CRE.ViewModels
{
    public class NonFundedResearchInfoViewModel
    { 
        public AppUserViewModel AppUser { get; set; }
        public ICollection<CoProponent> CoProponent { get; set; }
    }

}
