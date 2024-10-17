using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IInitialReviewServices
    {
        Task<IEnumerable<InitialReviewViewModel>> GetEthicsApplicationsForInitialReviewAsync();
        Task<InitialReviewViewModel> GetApplicationDetailsAsync(string urecNo);
        Task<IEnumerable<CoProponent>> GetCoProponentsByNonFundedResearchIdAsync(string nonFundedResearchId);
        Task<ReceiptInfo> GetReceiptInfoByUrecNoAsync(string urecNo);
        Task ApproveApplicationAsync(string urecNo, string comments);
        Task ReturnApplicationAsync(string urecNo, string comments);
        Task<IEnumerable<InitialReviewViewModel>> GetPendingApplicationsAsync();
        Task<IEnumerable<InitialReviewViewModel>> GetApprovedApplicationsAsync();

    }
}
