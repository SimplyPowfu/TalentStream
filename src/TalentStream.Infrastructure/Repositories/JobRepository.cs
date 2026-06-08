using Microsoft.EntityFrameworkCore;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Infrastructure.Persistence;

namespace TalentStream.Infrastructure.Repositories
{
	public class JobRepository : IJobRepository
	{
		private readonly SqlDbContext _context;

		public JobRepository(SqlDbContext context)
		{
			_context = context;
		}

		public async Task<JobPosting?> GetByIdAsync(int id)
		{
			return await _context.JobPostings.FirstOrDefaultAsync(j => j.Id == id);
		}

		public async Task<JobPosting?> GetByTitleAsync(string title)
		{
			return await _context.JobPostings.FirstOrDefaultAsync(j => j.Title == title);
		}

		public async Task AddAsync(JobPosting jobPosting)
		{
			await _context.JobPostings.AddAsync(jobPosting);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}