using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsEvaluatorExpertiseServices : IEthicsEvaluatorExpertiseServices
    {
        private readonly CREdbContext _context;
        public EthicsEvaluatorExpertiseServices(CREdbContext context)
        {
            _context = context;
        }
    }
}
