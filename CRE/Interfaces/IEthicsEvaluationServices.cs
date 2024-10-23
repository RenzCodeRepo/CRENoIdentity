using CRE.Models;

namespace CRE.Interfaces
{
    public interface IEthicsEvaluationServices
    {
        EthicsEvaluation GetEvaluationByUrecNo(string urecNo); 
        //Task UpdateEvaluation(EthicsEvaluation ethicsEvaluation);
        Task CreateEvaluation(EthicsEvaluation ethicsEvaluation);
    }
}
