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
			return await _context.Companies
			.Include(c => c.Employees)
			.Include(c => c.JobPostings)
			.FirstOrDefaultAsync(c => c.Name == name);
		}

		public void Update(Company company)
		{
			_context.Companies.Update(company);
		}

		public void Delete(Company company)
		{
			_context.Companies.Remove(company);
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