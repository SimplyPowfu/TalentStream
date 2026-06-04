using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.Company;

namespace TalentStream.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CompanyController : ControllerBase
	{
		private readonly ICompanyRepository _companyRepository;
		private readonly IUserRepository _userRepository;

		public CompanyController(ICompanyRepository companyRepository, IUserRepository userRepository)
		{
			_companyRepository = companyRepository;
			_userRepository = userRepository;
		}

		[HttpPost("register")]
		[Authorize(Roles = "Recruiter")]
		public async Task<IActionResult> Register(RegisterCompanyDto dto)
		{

			var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Utente non identificato nel token." });
        
            int userId = int.Parse(userIdClaim);
			
			var existingCompany = await _companyRepository.GetByNameAsync(dto.Name);
			if (existingCompany != null)
				return BadRequest(new { message = "Company gia' registrata." });
			
			var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Utente non trovato." });
            if (user.CompanyId != null)
                return BadRequest(new { message = "Questo account è già associato a un'azienda." });
			
			var newCompany = new Company
			{
				Name = dto.Name,
				Industry = dto.Industry,
				Location = dto.Location,
				CreatedAt = DateTime.UtcNow
			};

			await _companyRepository.AddAsync(newCompany);
			await _companyRepository.SaveChangesAsync();

			user.CompanyId = newCompany.Id;
			await _userRepository.SaveChangesAsync();

			return Ok(new { message = "Company creata con successo!"});
		}
	}
}