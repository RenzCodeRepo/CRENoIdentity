using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IInitialReviewServices
    {
        Task<IEnumerable<InitialReviewViewModel>> GetEthicsApplicationsForInitialReviewAsync();
    }
}
