using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.User.AnyAsync(u => u.userId == userId); // Assuming UserId is the primary key
        }
    }
}
