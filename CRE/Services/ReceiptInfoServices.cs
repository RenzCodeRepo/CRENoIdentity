using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ReceiptInfoServices : IReceiptInfoServices
    {
        private readonly ApplicationDbContext _context;
        public ReceiptInfoServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
