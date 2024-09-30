using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class InitialReviewServices : IInitialReviewServices
    {
        private readonly ApplicationDbContext _context;
        public InitialReviewServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
