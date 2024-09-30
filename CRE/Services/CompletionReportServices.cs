using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class CompletionReportServices : ICompletionReportServices
    {
        private readonly ApplicationDbContext _context;
        public CompletionReportServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
