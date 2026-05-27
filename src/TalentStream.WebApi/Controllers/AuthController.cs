using Microsoft.AspNetCore.Mvc;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.User;
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
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email già registrata." });

			var newUser = new User
            {
                Name = dto.Name,
                Surame = dto.Surname, 
                Email = dto.Email,
                PasswordHash = BCryptNet.HashPassword(dto.Password), 
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            // Prepariamo l'inserimento in memoria su EF
            await _userRepository.AddAsync(newUser);
            // Spingiamo i dati su SQL Server dentro Docker
            await _userRepository.SaveChangesAsync();

            return Ok(new { message = "Account creato con successo!" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser == null)
                return BadRequest(new { message = "Credenziali invalide."});
            if (!BCryptNet.Verify(dto.Password, existingUser.PasswordHash))
                return BadRequest(new { message = "Credenziali invalide."});
            existingUser.PasswordHash = string.Empty;
            return Ok(new { message = "Account recuperato con successo!", existingUser });
        }
    }
}