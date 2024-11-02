﻿using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IEthicsEvaluationServices
    {
        EthicsEvaluation GetEvaluationByUrecNo(string urecNo);
        Task<List<EthicsEvaluator>> GetAvailableEvaluatorsAsync(string fieldOfStudy);
        Task UpdateEvaluationStatusAsync(int evaluationId, string status, string? reasonForDecline);
        Task<int> CreateEvaluationAsync(EthicsEvaluation evaluation);
        Task<List<EthicsEvaluator>> GetAllEvaluatorsAsync();
        Task AssignEvaluatorAsync(string urecNo, int evaluatorId);
        Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo);
        Task<EvaluatedExemptApplication> GetEvaluationDetailsAsync(string urecNo, int evaluationId);
        Task SaveEvaluationAsync(EthicsEvaluation ethicsEvaluation);
        Task<EthicsEvaluation> GetEvaluationByUrecNoAndIdAsync(string urecNo, int evaluationId);
        Task<EthicsEvaluation> GetEvaluationByUrecNoAndEvaluatorIdAsync(string urecNo, int ethicsEvaluatorId);
        Task<List<string>> GetEvaluatedUrecNosAsync();
        Task<IEnumerable<AssignedEvaluationViewModel>> GetAssignedEvaluationsAsync(int evaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetAcceptedEvaluationsAsync(int evaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetCompletedEvaluationsAsync(int evaluatorId);
        Task<EthicsEvaluator> GetEvaluatorByUserIdAsync(string userId);
        Task UpdateEvaluationAsync(EthicsEvaluation ethicsEvaluation);
        Task<EvaluationDetailsViewModel> GetEvaluationAndEvaluatorDetailsAsync(string urecNo, int evaluationId);
        Task<IEnumerable<EthicsEvaluator>> GetAvailableEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string applicantFirstName, string applicantMiddleName, string applicantLastName);
        Task<IEnumerable<EthicsEvaluator>> GetRecommendedEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string requiredFieldOfStudy, string applicantFirstName, string applicantMiddleName, string applicantLastName);
        Task<List<ChiefEvaluationViewModel>> GetExemptApplicationsAsync();
        Task<List<EvaluatedExemptApplication>> GetEvaluatedExemptApplicationsAsync();
        Task<List<EvaluatedExpeditedApplication>> GetEvaluatedExpeditedApplicationsAsync();
        Task<List<EvaluatedFullReviewApplication>> GetEvaluatedFullReviewApplicationsAsync();
        Task IncrementDeclinedAssignmentCountAsync(int ethicsEvaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetDeclinedEvaluationsAsync(int evaluatorId);

    }
}
