using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsEvaluatorServices : IEthicsEvaluatorServices
    {
        private readonly CREdbContext _context;
        public EthicsEvaluatorServices(CREdbContext context)
        {
            _context = context;
        }
    }
}
