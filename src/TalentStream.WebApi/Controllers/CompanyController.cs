using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.Company;
using TalentStream.WebApi.Filter;

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
				return NotFound(new { message = "[POST] Utente non trovato." });
			if (user.CompanyId != null)
				return BadRequest(new { message = "Questo account è già associato a un'azienda." });

			var newCompany = new Company
			{
				Name = dto.Name,
				Industry = dto.Industry,
				Location = dto.Location ?? string.Empty,
				CreatedAt = DateTime.UtcNow
			};

			await _companyRepository.AddAsync(newCompany);
			await _companyRepository.SaveChangesAsync();

			user.CompanyId = newCompany.Id;
			await _userRepository.SaveChangesAsync();

			return Ok(new { message = "Company creata con successo!" });
		}

		[HttpGet("company")]
		[Authorize(Roles = "Recruiter")]
		[AuthorizeUser]
		public async Task<IActionResult> GetCompany()
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "[GET] Utente non trovato." });
			if (user.Company == null)
				return NotFound(new { message = "L'utente non correlato ad una Company." });
			var company = await _companyRepository.GetByNameAsync(user.Company.Name);
			if (company == null)
				return NotFound(new { message = "Company non trovata." });
			var response = new
			{
				id = company.Id,
				name = company.Name,
				employees = company.Employees?.Select(e => new
				{
					id = e.Id,
					name = e.Name,
					surname = e.Surname,
					email = e.Email,
					role = e.Role
				}).ToList(),
				jobPosting = company.JobPostings?.Select(j => new
				{
					id = j.Id,
					title = j.Title,
					description = j.Description,
					salaryRange = j.SalaryRange,
					createAt = j.CreatedAt
				}).ToList()
			};
			return Ok(new { message = "Company recuperata con successo!", response });
		}

		[HttpPatch("company")]
		[Authorize(Roles = "Recruiter")]
		[AuthorizeUser]
		public async Task<IActionResult> UpdateCompany(UpdateCompanyDto dto)
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "[PATCH] Utente non trovato." });
			if (user.Company == null)
				return NotFound(new { message = "L'utente non correlato ad una Company." });
			var company = await _companyRepository.GetByNameAsync(user.Company.Name);
			if (company == null)
				return NotFound(new { message = "Company non trovata." });
			company.Name = dto.Name ?? company.Name;
			company.Industry = dto.Industry ?? company.Industry;
			company.Location = dto.Location ?? company.Location;
			_companyRepository.Update(company);
			await _companyRepository.SaveChangesAsync();
			return Ok(new { message = "Company aggiornata con successo!" });
		}

		[HttpDelete("company")]
		[Authorize(Roles = "Recruiter")]
		[AuthorizeUser]
		public async Task<IActionResult> DeleteCompany()
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "[DELETE] Utente non trovato." });
			if (user.Company == null)
				return NotFound(new { message = "L'utente non correlato ad una Company." });
			var company = await _companyRepository.GetByNameAsync(user.Company.Name);
			if (company == null)
				return NotFound(new { message = "Company non trovata." });
			_companyRepository.Delete(company);
			await _companyRepository.SaveChangesAsync();
			return Ok(new { message = "Company eliminata con successo!" });
		}
	}
}