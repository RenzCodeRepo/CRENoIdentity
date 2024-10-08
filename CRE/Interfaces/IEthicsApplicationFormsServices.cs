using CRE.Models;

namespace CRE.Interfaces
{
    public interface IEthicsApplicationFormsServices
    {
        Task<IEnumerable<EthicsApplicationForms>> GetAllFormsByUrecAsync(string urecNo); 
        Task<EthicsForm> GetFormByIdAsync(string ethicsFormId);
        Task<EthicsApplicationForms> GetApplicationFormByIdAsync(int ethicsApplicationFormId);
        Task AddFormAsync(EthicsApplicationForms form);
        Task UpdateFormAsync(EthicsApplicationForms form);
        Task RemoveFormAsync(int ethicsApplicationFormId);
    }
}
