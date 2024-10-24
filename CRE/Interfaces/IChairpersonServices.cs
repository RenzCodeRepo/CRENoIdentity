using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IChairpersonServices
    {
        Task<List<EthicsApplication>> GetApplicationsByFieldOfStudyAsync(string userId);
        Task AssignEvaluatorsAsync(string urecNo, List<int> evaluatorIds);
        Task<EthicsApplication> GetApplicationAsync(string urecNo);
    }
}
