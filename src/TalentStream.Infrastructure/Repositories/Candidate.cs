using MongoDB.Driver;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Infrastructure.Persistence;

namespace TalentStream.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
		private readonly MongoDbContext _context;

		public CandidateRepository(MongoDbContext context)
		{
			_context = context;
		}

		public async Task<CandidateProfile?> GetByUserIdAsync(int userId)
		{
			return await _context.Candidates
			.Find(p => p.UserId == userId)
			.FirstOrDefaultAsync();
		}

		public async Task AddAsync(CandidateProfile profile)
		{
			await _context.Candidates.InsertOneAsync(profile);
		}
        public async Task UpdateAsync(CandidateProfile profile)
		{
			await _context.Candidates.ReplaceOneAsync(p => p.Id == profile.Id, profile);
		}
    }
}