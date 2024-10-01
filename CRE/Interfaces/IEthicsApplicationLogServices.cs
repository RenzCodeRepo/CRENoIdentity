using CRE.Models;

namespace CRE.Interfaces
{
    public interface IEthicsApplicationLogServices
    {
        Task AddLogAsync(EthicsApplicationLog log);
        Task<EthicsApplicationLog> GetLatestLogAsync(string urecNo); // Retrieve the latest log by date for a specific application
        Task<IEnumerable<EthicsApplicationLog>> GetLogsByUrecNoAsync(string urecNo); // Retrieve all logs for a specific application (urecNo) for tracking
    }
}
