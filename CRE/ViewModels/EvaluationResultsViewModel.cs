﻿using CRE.Models;

namespace CRE.ViewModels
{
    public class EvaluationResultsViewModel
    {
        public AppUser? AppUser { get; set; }
        public EthicsEvaluator? EthicsEvaluator { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public List<EthicsEvaluation> EthicsEvaluation { get; set; } = new List<EthicsEvaluation>(); // List for multiple evaluations
        public IEnumerable<EthicsApplicationForms>? ApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLog>? ApplicationLogs { get; set; }

        // Display properties for evaluation results
        public string ReviewerNames => string.Join(", ", EthicsEvaluation
             .Select(e => $"{e.EthicsEvaluator?.Faculty?.User?.fName} {e.EthicsEvaluator?.Faculty?.User?.mName} {e.EthicsEvaluator?.Faculty?.User?.lName}")
             .Where(name => !string.IsNullOrEmpty(name))); public string UrecNo => EthicsApplication?.urecNo;

        // Evaluation outcomes, grades, or remarks
        public List<string?> ProtocolResults => EthicsEvaluation.Select(e => e.ProtocolRecommendation).ToList();
        public List<string?> ConsentResults => EthicsEvaluation.Select(e => e.ConsentRecommendation).ToList();
        public List<DateOnly?> EvaluationDates => EthicsEvaluation.Select(e => e.endDate).ToList();

        // File references for any documents that were part of the evaluation
        public List<byte[]?> ProtocolReviewSheets => EthicsEvaluation.Select(e => e.ProtocolReviewSheet).ToList();
        public List<byte[]?> InformedConsentForms => EthicsEvaluation.Select(e => e.InformedConsentForm).ToList();
    }
}
