using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IEthicsEvaluationServices
    {
        EthicsEvaluation GetEvaluationByUrecNo(string urecNo);
        Task<List<EthicsEvaluator>> GetAvailableEvaluatorsAsync(string fieldOfStudy);
        Task UpdateEvaluationStatusAsync(int evaluationId, string status);
        Task CreateEvaluation(EthicsEvaluation ethicsEvaluation);
        Task AssignEvaluatorAsync(string urecNo, int evaluatorId);
        Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo);
        
    }
}
