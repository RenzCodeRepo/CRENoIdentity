using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class SecretariatServices : ISecretariatServices
    {
        private readonly CREdbContext _context;
        public SecretariatServices(CREdbContext context)
        {
            _context = context;
        }
    }
}
