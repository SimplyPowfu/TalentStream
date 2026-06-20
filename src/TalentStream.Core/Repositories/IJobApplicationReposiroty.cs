using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(int id);
        Task<IEnumerable<JobApplication>> GetByJobPostingIdAsync(int jobPostingId);
        Task<IEnumerable<JobApplication>> GetByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int userId, int jobPostingId); // Per evitare doppie candidature
        void Update(JobApplication application);
		void Delete(JobApplication application);
		Task AddAsync(JobApplication application);
        Task SaveChangesAsync();
    }
}