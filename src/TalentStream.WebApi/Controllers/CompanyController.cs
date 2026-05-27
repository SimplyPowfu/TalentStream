using Microsoft.AspNetCore.Mvc;
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

		public CompanyController(ICompanyRepository companyRepository)
		{
			_companyRepository = companyRepository;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterCompanyDto dto)
		{
			var existingCompany = await _companyRepository.GetByNameAsync(dto.Name);
			if (existingCompany != null)
				return BadRequest(new { message = "Company gia' registrata." });
			
			var newCompany = new Company
			{
				Name = dto.Name,
				Industry = dto.Industry,
				Location = dto.Location,
				CreatedAt = DateTime.UtcNow
			};

			await _companyRepository.AddAsync(newCompany);
			await _companyRepository.SaveChangesAsync();

			return Ok(new { message = "Company creata con successo!"});
		}
	}
}