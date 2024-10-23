using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using System;

namespace CRE.Services
{
    public class EthicsEvaluationServices : IEthicsEvaluationServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsEvaluationServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateEvaluation(EthicsEvaluation ethicsEvaluation)
        {
            _context.EthicsEvaluation.Add(ethicsEvaluation);
            _context.SaveChanges();
        }

        public EthicsEvaluation GetEvaluationByUrecNo(string urecNo)
        {
            return _context.EthicsEvaluation
          .FirstOrDefault(e => e.urecNo == urecNo);
        }

        //public async Task UpdateEvaluation(EthicsEvaluation ethicsEvaluation)
        //{
        //    var existingEvaluation = _context.EthicsEvaluation
        //   .FirstOrDefault(e => e.urecNo == ethicsEvaluation.urecNo);

        //    if (existingEvaluation != null)
        //    {
        //        // Update the existing evaluation
        //        existingEvaluation.Comments = ethicsEvaluation.Comments;
        //        existingEvaluation.Status = ethicsEvaluation.Status;
        //        // Update other fields as necessary

        //        _context.SaveChanges();
        //    }
        //}
    }
}
