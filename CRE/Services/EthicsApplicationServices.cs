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

        public async Task<string> GenerateUrecNoAsync()
        {
            string baseFormat = "UREC-{0}-{1}-{2}";
            string year = DateTime.Now.Year.ToString();
            string dayMonth = DateTime.Now.ToString("ddMM");

            string newUrecNo;
            int randomNumber;

            do
            {
                // Generate a random four-digit number
                randomNumber = new Random().Next(1000, 9999);
                newUrecNo = string.Format(baseFormat, year, dayMonth, randomNumber);
            }
            while (await IsUrecNoExistsAsync(newUrecNo));

            return newUrecNo;
        }

        public async Task<bool> IsUrecNoExistsAsync(string urecNo)
        {
            // Check if urecNo already exists for the current day
            return await _context.EthicsApplication
                .AnyAsync(a => a.urecNo == urecNo); // Assuming CreatedDate is a DateTime property in your model
        }

        public async Task<IEnumerable<EthicsApplication>> GetAllApplicationsAsync()
        {
            return await _context.EthicsApplication.ToListAsync();
        }


        public async Task<EthicsApplication> GetApplicationByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsApplication.FirstOrDefaultAsync(a => a.urecNo == urecNo);
        }

        public async Task<IEnumerable<EthicsApplication>> GetApplicationsByUserAsync(int userId)
        {
            return await _context.EthicsApplication
                .Where(a => a.userId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EthicsApplication>> GetApplicationsSortedByFieldOfStudyAsync()
        {
            return await _context.EthicsApplication
                .OrderBy(a => a.fieldOfStudy)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
