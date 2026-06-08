using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
	public interface IUserRepository
	{
		Task<User?> GetByEmailAsync(string email);
		Task<User?> GetByIdAsync(int id);
		Task AddAsync(User user);
		Task SaveChangesAsync();
	}
}