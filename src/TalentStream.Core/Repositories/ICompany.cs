using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
	public interface ICompanyRepository
	{
		Task<Company?> GetByNameAsync(string email);
		Task AddAsync(Company company);
		Task SaveChangesAsync();
	}
}