using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
	public interface ICompanyRepository
	{
		Task<Company?> GetByNameAsync(string name);
		void Update(Company company);

		void Delete(Company company);
		Task AddAsync(Company company);
		Task SaveChangesAsync();
	}
}