using CRE.Models;

namespace CRE.Interfaces
{
    public interface IAppUserServices
    {
        Task<AppUser> GetByIdAsync(string userId);
        //Task<bool> UserExistsAsync(int userId); // Check if a user exists
    }
}
