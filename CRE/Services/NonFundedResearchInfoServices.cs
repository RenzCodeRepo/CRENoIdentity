using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRE.Services
{
    public class NonFundedResearchInfoServices : INonFundedResearchInfoServices
    {
        private readonly ApplicationDbContext _context;

        public NonFundedResearchInfoServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddNonFundedResearchAsync(NonFundedResearchInfo research)
        {
            _context.NonFundedResearchInfo.Add(research);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteNonFundedResearchAsync(string researchId)
        {
            var research = await _context.NonFundedResearchInfo.FindAsync(researchId);
            if (research != null)
            {
                _context.NonFundedResearchInfo.Remove(research);
                await _context.SaveChangesAsync();
            }
        }

        // Method to generate a unique primary key (NFID-XXXX)
        public async Task<string> GenerateNonFundedResearchIdAsync()
        {
            string id;
            bool exists;
            Random random = new Random();

            do
            {
                // Generate the ID in the format NFID-XXXX, where XXXX are 4 random digits
                id = "NFID-" + random.Next(1000, 9999);

                // Check if the generated ID already exists in the database
                exists = await _context.NonFundedResearchInfo
                                       .AnyAsync(r => r.nonFundedResearchId == id);

            } while (exists); // Keep generating until a unique ID is found

            return id;
        }

        public async Task<IEnumerable<NonFundedResearchInfo>> GetAllNonFundedResearchAsync()
        {
            return await _context.NonFundedResearchInfo.ToListAsync();
        }

        public async Task<NonFundedResearchInfo> GetNonFundedResearchByUrecNoAsync(string urecNo)
        {
            return await _context.NonFundedResearchInfo
                .FirstOrDefaultAsync(r => r.urecNo == urecNo);
        }

        public async Task<IEnumerable<NonFundedResearchInfo>> GetNonFundedResearchByUserAsync(string userId)
        {
            return await _context.NonFundedResearchInfo
                //.Where(r => r.userId == userId) // Assuming UserId is the foreign key to the User
                .ToListAsync();
        }

        public async Task<NonFundedResearchInfo> SearchByTitleAsync(string title)
        {
            string normalizedTitle = title.ToLower().Replace(" ", ""); // Normalize title by lowering case and removing spaces
            return await _context.NonFundedResearchInfo
                                 .FirstOrDefaultAsync(r => r.title.ToLower().Replace(" ", "") == normalizedTitle);
        }
        public async Task UpdateNonFundedResearchAsync(NonFundedResearchInfo research)
        {
            _context.NonFundedResearchInfo.Update(research);
            await _context.SaveChangesAsync();
        }
    }
}
