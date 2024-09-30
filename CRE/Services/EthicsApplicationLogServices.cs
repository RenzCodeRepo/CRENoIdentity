using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsApplicationLogServices : IEthicsApplicationLogServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsApplicationLogServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
