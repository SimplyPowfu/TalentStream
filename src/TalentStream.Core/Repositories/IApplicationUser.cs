using TalentStream.Core.Entities;

namespace TalentStream.Core.Repositories
{
	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(int id);
		Task<User?> GetByEmailAsync(string email);
		void Update(User user);
		void Delete(User user);
		Task AddAsync(User user);
		Task SaveChangesAsync();
	}
}