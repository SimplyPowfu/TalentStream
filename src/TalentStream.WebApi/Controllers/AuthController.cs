using Microsoft.AspNetCore.Mvc;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using BCryptNet = BCrypt.Net.BCrypt;

namespace TalentStream.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email già registrata." });
            }

			user.PasswordHash = BCryptNet.HashPassword(user.PasswordHash);

            user.CreatedAt = DateTime.UtcNow;

            // Prepariamo l'inserimento in memoria su EF
            await _userRepository.AddAsync(user);

            // Spingiamo fisicamente i dati su SQL Server dentro Docker
            await _userRepository.SaveChangesAsync();

            return Ok(new { message = "Account creato con successo!", userId = user.Id });
        }
    }
}