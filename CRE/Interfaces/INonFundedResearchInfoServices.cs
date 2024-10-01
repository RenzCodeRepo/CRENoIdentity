using CRE.Models;

namespace CRE.Interfaces
{
    public interface INonFundedResearchInfoServices
    {
        Task AddNonFundedResearchAsync(NonFundedResearchInfo research);
        Task UpdateNonFundedResearchAsync(NonFundedResearchInfo research);
        Task DeleteNonFundedResearchAsync(string researchId);
        Task<NonFundedResearchInfo> SearchByTitleAsync(string title);
        Task<IEnumerable<NonFundedResearchInfo>> GetAllNonFundedResearchAsync();
        Task<IEnumerable<NonFundedResearchInfo>> GetNonFundedResearchByUserAsync(int userId);
    }
}
