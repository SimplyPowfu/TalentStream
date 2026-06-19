using Microsoft.EntityFrameworkCore;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Infrastructure.Persistence;

namespace TalentStream.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly SqlDbContext _context;

		public UserRepository(SqlDbContext context)
		{
			_context = context;
		}

		public async Task<User?> GetByIdAsync(int id)
		{
			return await _context.Users
			.Include(u => u.Company)
			.FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<User?> GetByEmailAsync(string email)
		{
			return await _context.Users
			.Include(u => u.Company)
			.FirstOrDefaultAsync(u => u.Email == email);
		}

		public void Update(User user)
		{
			_context.Users.Update(user);
		}

		public void Delete(User user)
		{
			_context.Users.Remove(user);
		}

		public async Task AddAsync(User user)
		{
			await _context.Users.AddAsync(user);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}