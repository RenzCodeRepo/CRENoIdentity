using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class EthicsApplicationServices : IEthicsApplicationServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsApplicationServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ApplyForEthicsAsync(EthicsApplication application)
        {
            _context.EthicsApplication.Add(application);
            await _context.SaveChangesAsync();
        }

        public async Task CancelApplicationAsync(string urecNo)
        {
            var application = await GetApplicationByUrecNoAsync(urecNo);
            if (application != null)
            {
                _context.EthicsApplication.Remove(application);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EditApplicationAsync(EthicsApplication application)
        {
            _context.EthicsApplication.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EthicsApplication>> GetAllApplicationsAsync()
        {
            return await _context.EthicsApplication.ToListAsync();
        }


        public async Task<EthicsApplication> GetApplicationByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsApplication.FirstOrDefaultAsync(a => a.urecNo == urecNo);
        }

        public async Task<IEnumerable<EthicsApplication>> GetApplicationsSortedByFieldOfStudyAsync()
        {
            return await _context.EthicsApplication
                .OrderBy(a => a.fieldOfStudy)
                .ToListAsync();
        }
    }
}
