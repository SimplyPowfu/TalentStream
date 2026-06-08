using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.JobPosting;

namespace TalentStream.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JobPostingController : ControllerBase
	{
		private readonly IJobRepository _jobRepository;
		private readonly IUserRepository _userRepository;

		public JobPostingController(IJobRepository jobRepository, IUserRepository userRepository)
		{
			_jobRepository = jobRepository;
			_userRepository = userRepository;
		}

		[HttpPost("register")]
		[Authorize(Roles = "Recruiter")]
		public async Task<IActionResult> Register(RegisterJobDto dto)
		{
			var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userIdClaim))
				return Unauthorized(new { message = "Utente non identificato nel token." });
			int userId = int.Parse(userIdClaim);

			var existingJobPost = await _jobRepository.GetByTitleAsync(dto.Title);
			if (existingJobPost != null)
				return BadRequest(new { message = "JobPost gia' registrata." });

			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });
			if (user.CompanyId == null)
				return BadRequest(new { message = "Questo account non è associato a un'azienda." });

			var newJobPost = new JobPosting
			{
				Title = dto.Title,
				Description = dto.Description,
				SalaryRange = dto.SalaryRange,
				CreatedAt = DateTime.UtcNow
			};
			newJobPost.CompanyId = (int)user.CompanyId;

			await _jobRepository.AddAsync(newJobPost);
			await _jobRepository.SaveChangesAsync();
			return Ok(new { message = "JobPost creata con successo!" });
		}


	}
}