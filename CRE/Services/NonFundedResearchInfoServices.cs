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
        public async Task<IEnumerable<NonFundedResearchInfo>> GetAllNonFundedResearchAsync()
        {
            return await _context.NonFundedResearchInfo.ToListAsync();
        }

        public async Task<IEnumerable<NonFundedResearchInfo>> GetNonFundedResearchByUserAsync(int userId)
        {
            return await _context.NonFundedResearchInfo
                .Where(r => r.userId == userId) // Assuming UserId is the foreign key to the User
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
