using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsEvaluatorServices : IEthicsEvaluatorServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsEvaluatorServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
