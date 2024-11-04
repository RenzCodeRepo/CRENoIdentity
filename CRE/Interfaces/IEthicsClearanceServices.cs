using CRE.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRE.Interfaces
{
    public interface IEthicsClearanceServices
    {
        Task<bool> IssueEthicsClearanceAsync(EthicsClearance ethicsClearance, IFormFile uploadedFile, string remarks, string userId);
        Task<EthicsClearance> GetClearanceByUrecNoAsync(string urecNo);
    }
}
