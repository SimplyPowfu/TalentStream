using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
    public interface ICandidateRepository
    {
        Task<CandidateProfile?> GetByIdAsync(string id);
        Task<IEnumerable<CandidateProfile>> GetAllAsync();
        Task AddAsync(CandidateProfile candidate);
        Task UpdateAsync(CandidateProfile candidate);
        Task DeleteAsync(string id);
    }
}