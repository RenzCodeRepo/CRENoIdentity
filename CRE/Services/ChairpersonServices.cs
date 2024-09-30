using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ChairpersonServices : IChairpersonServices
    {
        private readonly ApplicationDbContext _context;

        public ChairpersonServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
