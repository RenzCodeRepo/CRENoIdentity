using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsApplicationServices : IEthicsApplicationServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsApplicationServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
