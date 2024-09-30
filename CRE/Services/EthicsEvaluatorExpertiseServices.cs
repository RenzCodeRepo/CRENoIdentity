using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsEvaluatorExpertiseServices : IEthicsEvaluatorExpertiseServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsEvaluatorExpertiseServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
