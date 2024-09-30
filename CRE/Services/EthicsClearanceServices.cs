using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsClearanceServices : IEthicsClearanceServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsClearanceServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
