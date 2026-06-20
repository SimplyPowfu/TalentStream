using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Infrastructure.Persistence;

namespace TalentStream.WebApi.Controllers
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly SqlDbContext _context;

        public JobApplicationRepository(SqlDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications
                .Include(a => a.JobPosting)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<JobApplication>> GetByJobPostingIdAsync(int jobPostingId)
        {
            return await _context.JobApplications
                .Include(a => a.User)
                .Where(a => a.JobPostingId == jobPostingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetByUserIdAsync(int userId)
        {
            return await _context.JobApplications
                .Include(a => a.JobPosting)
                    .ThenInclude(j => j.Company) // Per far vedere al candidato il nome dell'azienda
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int userId, int jobPostingId)
        {
            return await _context.JobApplications
                .AnyAsync(a => a.UserId == userId && a.JobPostingId == jobPostingId);
        }

		public void Update(JobApplication application)
		{
			_context.JobApplications.Update(application);
		}
		public void Delete(JobApplication application)
		{
			_context.JobApplications.Remove(application);
		}

		 public async Task AddAsync(JobApplication application)
        {
            await _context.JobApplications.AddAsync(application);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}