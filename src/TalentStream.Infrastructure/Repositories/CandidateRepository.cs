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

        public async Task<CandidateProfile?> GetByIdAsync(string id)
        {
            return await _context.Candidates
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CandidateProfile>> GetAllAsync()
        {
            return await _context.Candidates
                .Find(_ => true) // Cerca tutti i documenti senza filtri
                .ToListAsync();
        }

        public async Task AddAsync(CandidateProfile candidate)
        {
            await _context.Candidates.InsertOneAsync(candidate);
        }

        public async Task UpdateAsync(CandidateProfile candidate)
        {
            // Rimpiazza il documento esistente che ha lo stesso ID
            await _context.Candidates
                .ReplaceOneAsync(c => c.Id == candidate.Id, candidate);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.Candidates.DeleteOneAsync(c => c.Id == id);
        }
    }
}