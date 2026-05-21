using Microsoft.EntityFrameworkCore;
using TalentStream.Core.Entities;

namespace TalentStream.Infrastructure.Persistence
{
    public class SqlDbContext : DbContext
    {
        // Il costruttore riceve le opzioni di configurazione (es. la stringa di connessione)
        // dalla WebApi e le passa alla classe base DbContext.
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
        }

        // I DbSet dicono a EF Core quali classi devono diventare tabelle nel Database.
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<JobPosting> JobPostings { get; set; } = null!;

        // Usiamo questo metodo per rifinire le regole delle colonne (es. precisione dei decimali o vincoli).
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SQL Server ha bisogno di sapere quante cifre totali e quanti decimali gestire.
            // 18,2 significa: 18 cifre totali di cui 2 decimali (es. 150000.00).
            modelBuilder.Entity<JobPosting>()
                .Property(j => j.SalaryRange)
                .HasColumnType("decimal(18,2)");
        }
    }
}