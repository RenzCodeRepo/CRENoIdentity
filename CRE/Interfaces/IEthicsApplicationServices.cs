using CRE.Models;

namespace CRE.Interfaces
{
    public interface IEthicsApplicationServices
    {
        Task<IEnumerable<EthicsApplication>> GetAllApplicationsAsync(); // Retrieve all applications for reporting or filtering
        Task<EthicsApplication> GetApplicationByUrecNoAsync(string urecNo); // Get a specific application by unique record number
        Task ApplyForEthicsAsync(EthicsApplication application); // Submit a new ethics application
        Task EditApplicationAsync(EthicsApplication application); // Edit existing application information
        Task CancelApplicationAsync(string urecNo); // Cancel an application / Deletes the record
        Task<IEnumerable<EthicsApplication>> GetApplicationsSortedByFieldOfStudyAsync(); // Get applications sorted by field of study
        Task<IEnumerable<EthicsApplication>> GetApplicationsByUserAsync(int userId); // Get all applications submitted by a specific user
        Task<string> GenerateUrecNoAsync(); // Generate a unique record number for new applications
        Task<bool> IsUrecNoExistsAsync(string urecNo);
    }
}
