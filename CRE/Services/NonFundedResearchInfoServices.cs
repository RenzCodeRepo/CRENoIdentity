using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class NonFundedResearchInfoServices : INonFundedResearchInfoServices
    {
        private readonly ApplicationDbContext _context;
        public NonFundedResearchInfoServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
