using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class SecretariatServices : ISecretariatServices
    {
        private readonly ApplicationDbContext _context;
        public SecretariatServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
