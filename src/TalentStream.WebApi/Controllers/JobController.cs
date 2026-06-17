using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.JobPosting;
using TalentStream.WebApi.Filters;

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

		[HttpGet("alljob")]
		[Authorize(Roles = "Candidate,Recruiter")]
		public async Task<IActionResult> AllJobs()
		{
			var jobPostings = await _jobRepository.GetAllJobPostingAsync();
			var response = jobPostings.Select(job => new GetJobResponseDto
			{
				Id = job.Id,
				Title = job.Title,
				Description = job.Description,
				SalaryRange = job.SalaryRange,
				CompanyId = job.CompanyId,
				CompanyName = job.Company?.Name ?? "Azienda Sconosciuta",
				CompanyLocation = job.Company?.Location ?? "Remoto"
			});
			return Ok(response);
		}

		[HttpGet("job/{id}")]
		[Authorize(Roles = "Candidate,Recruiter")]
		public async Task<IActionResult> GetIdJob(int id)
		{
			var job = await _jobRepository.GetIdJobPostingAsync(id);
			if (job == null)
				return NotFound(new { message = "JobPost non trovata." });
			var response = new GetJobResponseDto
			{
				Id = job.Id,
				Title = job.Title,
				Description = job.Description,
				SalaryRange = job.SalaryRange,
				CompanyId = job.CompanyId,
				CompanyName = job.Company?.Name ?? "Azienda Sconosciuta",
				CompanyLocation = job.Company?.Location ?? "Remoto"
			};
			return Ok(response);
		}

		[HttpPatch("job/{id}")]
		[Authorize(Roles = "Recruiter")]
		[AuthorizeJobOwner]
		public async Task<IActionResult> UpdateIdJob(int id, UpdateJobDto dto)
		{
			var job = await _jobRepository.GetIdJobPostingAsync(id);
			if (job == null)
				return NotFound(new { message = "JobPost non trovata." });

			job.Title = string.IsNullOrEmpty(dto.Title) ? job.Title : dto.Title;
			job.Description = string.IsNullOrEmpty(dto.Description) ? job.Description : dto.Description;
			job.SalaryRange = dto.SalaryRange ?? job.SalaryRange;
			_jobRepository.Update(job);
			await _jobRepository.SaveChangesAsync();

			return Ok(new { message = "Annuncio aggiornato con successo!"});
		}

		[HttpDelete("job/{id}")]
		[Authorize(Roles = "Recruiter")]
		[AuthorizeJobOwner]
		public async Task<IActionResult> DeleteIdJob(int id)
		{
			var job = await _jobRepository.GetIdJobPostingAsync(id);
			if (job == null)
				return NotFound(new { message = "JobPost non trovata." });
			_jobRepository.Delete(job);
			await _jobRepository.SaveChangesAsync();

			return Ok(new { message = "Annuncio Cancellato con successo!"});
		}
	}
}