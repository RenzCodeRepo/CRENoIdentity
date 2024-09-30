using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;
        public UserServices(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
