using Microsoft.EntityFrameworkCore;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Infrastructure.Persistence;

namespace TalentStream.Infrastructure.Repositories
{
	public class CompanyRepository : ICompanyRepository
	{
		private readonly SqlDbContext _context;

		public CompanyRepository(SqlDbContext context)
		{
			_context = context;
		}

		public async Task<Company?> GetByNameAsync(string name)
		{
			return await _context.Companies.FirstOrDefaultAsync(c => c.Name == name);
		}

		public async Task AddAsync(Company company)
		{
			await _context.Companies.AddAsync(company);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}