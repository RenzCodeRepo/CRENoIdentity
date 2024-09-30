using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class CoProponentServices : ICoProponentServices
    {
        private readonly ApplicationDbContext _context;
        public CoProponentServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
