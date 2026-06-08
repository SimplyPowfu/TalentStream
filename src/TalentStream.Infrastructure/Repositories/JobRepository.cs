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

		public async Task<IEnumerable<JobPosting>> GetAllJobPostingAsync()
		{
			return await _context.JobPostings
			.Include(j => j.Company) //JOIN SQL con la tabella Companies
			.OrderByDescending(j => j.CreatedAt)
			.ToListAsync();
		}

		public async Task<JobPosting?> GetIdJobPostingAsync(int id)
		{
			return await _context.JobPostings
			.Include(j => j.Company)
			.FirstOrDefaultAsync(j => j.Id == id);
		}

		public void Update(JobPosting jobPosting)
		{
			_context.JobPostings.Update(jobPosting);
		}

		public void Delete(JobPosting jobPosting)
		{
			_context.JobPostings.Remove(jobPosting);
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