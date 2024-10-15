using CRE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CRE.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Chairperson> Chairperson { get; set; }
        public DbSet<Chief> Chief { get; set; }
        public DbSet<CoProponent> CoProponent { get; set; }
        public DbSet<CompletionCertificate> CompletionCertificate { get; set; }
        public DbSet<CompletionReport> CompletionReport { get; set; }
        public DbSet<EthicsApplication> EthicsApplication { get; set; }
        public DbSet<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public DbSet<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public DbSet<EthicsClearance> EthicsClearance { get; set; }
        public DbSet<EthicsEvaluation> EthicsEvaluation { get; set; }
        public DbSet<EthicsEvaluator> EthicsEvaluator { get; set; }
        public DbSet<EthicsEvaluatorExpertise> EthicsEvaluatorExpertise { get; set; }
        public DbSet<EthicsForm> EthicsForm { get; set; }
        public DbSet<Expertise> Expertise { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<NonFundedResearchInfo> NonFundedResearchInfo { get; set; }
        public DbSet<InitialReview> InitialReview { get; set; }
        public DbSet<ReceiptInfo> ReceiptInfo { get; set; }
        public DbSet<Secretariat> Secretariat { get; set; }
        public DbSet<AppUser> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship and restrict delete behavior for 'E_Clearance' and 'Gen_Info_NF_Research'
            modelBuilder.Entity<NonFundedResearchInfo>()
                .HasOne(g => g.EthicsClearance) // Gen_Info_NF_Research has one E_Clearance
                .WithOne(e => e.NonFundedResearchInfo) // E_Clearance has one Gen_Info_NF_Research
                .HasForeignKey<NonFundedResearchInfo>(g => g.ethicsClearanceId) // e_Clearance_Id is the foreign key in Gen_Info_NF_Research
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Define composite key for E_Evaluator_Expertise
            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasKey(ee => new { ee.ethicsEvaluatorId, ee.expertiseId });
            // Define composite key for E_Evaluator_Expertise
            modelBuilder.Entity<EthicsEvaluation>()
                .HasOne(e => e.EthicsEvaluator) // Assuming `E_Evaluation` has a foreign key to `E_Evaluator`
                .WithMany(ev => ev.EthicsEvaluation) // Assuming one `E_Evaluator` can have many `E_Evaluations`
                .HasForeignKey(e => e.ethicsEvaluatorId)  // Foreign key in `E_Evaluation` that points to `E_Evaluator`
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasOne(ee => ee.EthicsEvaluator)
                .WithMany(ev => ev.EthicsEvaluatorExpertise)
                .HasForeignKey(ee => ee.ethicsEvaluatorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<EthicsEvaluatorExpertise>()
                .HasOne(ee => ee.Expertise)
                .WithMany(ex => ex.EthicsEvaluatorExpertise)
                .HasForeignKey(ee => ee.expertiseId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Existing relationships should stay the same
            modelBuilder.Entity<NonFundedResearchInfo>()
                .HasOne(g => g.User)
                .WithMany(u => u.NonFundedResearchInfo)
                .HasForeignKey(g => g.userId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EthicsApplicationLog>()
                .HasOne(e => e.User)
                .WithMany(u => u.EthicsApplicationLog)
                .HasForeignKey(e => e.userId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EthicsApplicationLog>()
                .HasOne(e => e.EthicsApplication)
                .WithMany(u => u.EthicsApplicationLog) // Adjust based on the actual relationship
                .HasForeignKey(e => e.urecNo); // Explicitly set urec_No as the FK

            modelBuilder.Entity<InitialReview>()
                .HasOne(ir => ir.Secretariat)
                .WithMany(s => s.InitialReview) // assuming Secretariat has a collection of Initial_Reviews
                .HasForeignKey(ir => ir.secretariatId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
            base.OnModelCreating(modelBuilder);
        }
    }
}
