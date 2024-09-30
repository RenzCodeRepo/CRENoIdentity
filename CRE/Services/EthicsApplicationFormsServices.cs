using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsApplicationFormsServices : IEthicsApplicationFormsServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsApplicationFormsServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
