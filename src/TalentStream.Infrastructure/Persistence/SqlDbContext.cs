using Microsoft.EntityFrameworkCore;
using TalentStream.Core.Entities;

namespace TalentStream.Infrastructure.Persistence
{
	public class SqlDbContext : DbContext
	{
		public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; } = null!;
		public DbSet<Company> Companies { get; set; } = null!;
		public DbSet<JobPosting> JobPostings { get; set; } = null!;
		public DbSet<JobApplication> JobApplications { get; set; } = null!;

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