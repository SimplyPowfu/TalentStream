using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
	public interface ICompanyRepository
	{
		Task<Company?> GetByNameAsync(string name);
		Task AddAsync(Company company);
		Task SaveChangesAsync();
	}
}