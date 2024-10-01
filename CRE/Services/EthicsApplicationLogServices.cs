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
    public class EthicsApplicationLogServices : IEthicsApplicationLogServices
    {
        private readonly ApplicationDbContext _context;

        public EthicsApplicationLogServices(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add a new log with user input for status and comments
        public async Task AddLogAsync(EthicsApplicationLog log)
        {
            _context.EthicsApplicationLog.Add(log); // Add the log to the DbSet
            await _context.SaveChangesAsync();  // Commit the changes to the database
        }

        // Retrieve the latest log for a specific urecNo
        public async Task<EthicsApplicationLog> GetLatestLogAsync(string urecNo)
        {
            return await _context.EthicsApplicationLog
                                 .Where(log => log.urecNo == urecNo)        // Filter by urecNo
                                 .OrderByDescending(log => log.changeDate)   // Order by changeDate (latest first)
                                 .FirstOrDefaultAsync();                    // Get the latest log entry
        }

        // Retrieve all logs for a specific urecNo
        public async Task<IEnumerable<EthicsApplicationLog>> GetLogsByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsApplicationLog
                                 .Where(log => log.urecNo == urecNo)       // Filter by urecNo
                                 .OrderBy(log => log.changeDate)            // Order by changeDate (oldest to latest)
                                 .ToListAsync();                           // Return all matching logs
        }
    }
}
