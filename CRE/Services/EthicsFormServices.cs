using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsFormServices : IEthicsFormServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsFormServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
