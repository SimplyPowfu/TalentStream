using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.User;
using BCryptNet = BCrypt.Net.BCrypt;
using TalentStream.WebApi.Filter;

namespace TalentStream.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly IConfiguration _configuration;

		public AuthController(IUserRepository userRepository, IConfiguration configuration)
		{
			_userRepository = userRepository;
			_configuration = configuration;
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
				Surname = dto.Surname,
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
				return BadRequest(new { message = "Credenziali invalide." });
			if (!BCryptNet.Verify(dto.Password, existingUser.PasswordHash))
				return BadRequest(new { message = "Credenziali invalide." });

			var tokenHandler = new JwtSecurityTokenHandler();
			var secretKey = _configuration["jwt:Secret"];
			var key = Encoding.ASCII.GetBytes(secretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
					new Claim(ClaimTypes.Email, existingUser.Email),
					new Claim(ClaimTypes.Role, existingUser.Role)
				}),
				Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["jwt:ExpiryInMinutes"])),
				Issuer = _configuration["jwt:Issuer"],
				Audience = _configuration["jwt:Audience"],
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			existingUser.PasswordHash = string.Empty;
			var UserResponse = new
			{
				id = existingUser.Id,
				name = existingUser.Name,
				surname = existingUser.Surname,
				email = existingUser.Email,
				role = existingUser.Role,
				companyName = existingUser.Company?.Name
			};

			return Ok(new
			{
				message = "Login effettuato con successo!",
				token = tokenString,
				UserResponse
			});
		}

		[HttpGet("User")]
		[AuthorizeUser]
		public ActionResult<User> GetUser()
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });
			var response = new
			{
				name = user.Name,
				surname = user.Surname,
				email = user.Email,
				role = user.Role,
				company = user.Company?.Name ?? null
			};
			return Ok(new { message = "Account recuperato con successo", response });
		}

		[HttpPatch("User")]
		[AuthorizeUser]
		public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });
			user.Name = dto.Name ?? user.Name;
			user.Surname = dto.Surname ?? user.Surname;
			user.Email = dto.Email ?? user.Email;
			if (!string.IsNullOrEmpty(dto.Password))
				user.PasswordHash = BCryptNet.HashPassword(dto.Password);
			_userRepository.Update(user);
			await _userRepository.SaveChangesAsync();
			return Ok(new { message = "Account aggiornato con successo" });
		}

		[HttpDelete("User")]
		[AuthorizeUser]
		public async Task<IActionResult> DeleteUser()
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });
			_userRepository.Delete(user);
			await _userRepository.SaveChangesAsync();
			return Ok(new { message = "Account Eliminato con successo" });
		}
	}
}