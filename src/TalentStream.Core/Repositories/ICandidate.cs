using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
    public interface ICandidateRepository
    {
		Task<CandidateProfile?> GetByUserIdAsync(int userId);
        Task AddAsync(CandidateProfile profile);
        Task UpdateAsync(CandidateProfile profile);
		
		Task<bool> Delete(int userId);
    }
}