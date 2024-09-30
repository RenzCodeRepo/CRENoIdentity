using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsEvaluationServices : IEthicsEvaluationServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsEvaluationServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
