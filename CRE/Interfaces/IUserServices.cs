using CRE.Models;

namespace CRE.Interfaces
{
    public interface IUserServices
    {
        Task<User> GetByIdAsync(int userId); 
    }
}
