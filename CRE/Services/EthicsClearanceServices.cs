using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class EthicsClearanceServices : IEthicsClearanceServices
    {
        private readonly ApplicationDbContext _context;
        public EthicsClearanceServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EthicsClearance> GetClearanceByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsClearance
            .FirstOrDefaultAsync(clearance => clearance.urecNo == urecNo);
        }

        public async Task<bool> IssueEthicsClearanceAsync(EthicsClearance ethicsClearance, IFormFile uploadedFile, string remarks, string userId)
        {
            // Check if there's an uploaded file to process
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await uploadedFile.CopyToAsync(memoryStream); // Copy file to memory stream
                    ethicsClearance.file = memoryStream.ToArray(); // Store the file as byte array
                }
            }

            // Add the EthicsClearance record to the database
            _context.EthicsClearance.Add(ethicsClearance);

            // Create a new log entry for the issuance process
            var applicationLog = new EthicsApplicationLog
            {
                urecNo = ethicsClearance.urecNo,
                status = "Clearance Issued",
                comments = remarks,              
                changeDate = DateTime.Now,
                userId = userId 
            };

            // Add the log entry to the logs table
            _context.EthicsApplicationLog.Add(applicationLog);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true; // Return true if the operation is successful
        }

    }
}
