using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ChiefServices : IChiefServices
    {
        private readonly ApplicationDbContext _context;
        public ChiefServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
