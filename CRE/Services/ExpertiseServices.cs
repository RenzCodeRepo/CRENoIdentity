using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ExpertiseServices: IExpertiseServices
    {
        private readonly ApplicationDbContext _context;
        public ExpertiseServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
