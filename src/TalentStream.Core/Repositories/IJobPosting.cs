using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
    public interface IJobRepository
    {
        Task<JobPosting?> GetByIdAsync(int id);
		Task<JobPosting?> GetByTitleAsync(string title);
        Task AddAsync(JobPosting jobPosting);
        Task SaveChangesAsync();
    }
}