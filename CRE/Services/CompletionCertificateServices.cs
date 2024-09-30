using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class CompletionCertificateServices : ICompletionCertificateServices
    {
        private readonly ApplicationDbContext _context;
        public CompletionCertificateServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
