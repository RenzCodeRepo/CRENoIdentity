using CRE.Data;
using CRE.Interfaces;
using CRE.Models;

namespace CRE.Services
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;
        public UserServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.User.FindAsync(userId); // Retrieve user by user ID
        }

    }
}
