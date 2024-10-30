﻿using CRE.Data;
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
            string baseFormat = "UREC-{0}-{1}"; // "UREC-YYYY-XXXX"
            string year = DateTime.Now.Year.ToString();

            // Fetch existing UREC Nos for the current year
            var existingUrecNos = await _context.EthicsApplication
                .Where(u => u.urecNo.StartsWith($"UREC-{year}-"))
                .Select(u => u.urecNo)
                .ToListAsync();

            // Determine the next sequence number
            int newSequenceNumber = 1; // Default to 1
            if (existingUrecNos.Any())
            {
                // Extract the last four digits from existing UREC Nos
                var lastNumbers = existingUrecNos.Select(u =>
                    int.Parse(u.Substring(u.Length - 4))).ToList();

                // Get the maximum number and increment it
                newSequenceNumber = lastNumbers.Max() + 1;
            }

            // Format the new UREC No.
            string newUrecNo = string.Format(baseFormat, year, newSequenceNumber.ToString("D4"));

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

        public async Task<IEnumerable<EthicsApplication>> GetApplicationsByUserAsync(string userId)
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

        public async Task<EthicsApplication> GetApplicationByDtsNoAsync(string dtsNo)
        {
            return await _context.EthicsApplication
                                 .FirstOrDefaultAsync(e => e.dtsNo == dtsNo);
        }

    }
}
