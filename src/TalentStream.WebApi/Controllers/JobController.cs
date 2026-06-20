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
		public async Task<IActionResult> AllJobs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1 || pageSize > 20) pageSize = 10;

			var (jobPostings, totalRecords) = await _jobRepository.GetPaginatedJobPostingsAsync(pageNumber, pageSize);

			var data = jobPostings.Select(job => new GetJobResponseDto
			{
				Id = job.Id,
				Title = job.Title,
				Description = job.Description,
				SalaryRange = job.SalaryRange,
				CompanyId = job.CompanyId,
				CompanyName = job.Company?.Name ?? "Azienda Sconosciuta",
				CompanyLocation = job.Company?.Location ?? "Remoto"
			});
			var response = new 
			{
				Metadata = new 
				{
					CurrentPage = pageNumber,
					PageSize = pageSize,
					TotalRecords = totalRecords,
					TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
				},
				Data = data
			};

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
			var job = HttpContext.Items["ValidatedJob"] as JobPosting;
			if (job == null)
				return NotFound(new { message = "Errore nel recupero della JobPost" });

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
			var job = HttpContext.Items["ValidatedJob"] as JobPosting;
			if (job == null)
				return NotFound(new { message = "Errore nel recupero della JobPost" });
			_jobRepository.Delete(job);
			await _jobRepository.SaveChangesAsync();

			return Ok(new { message = "Annuncio Cancellato con successo!"});
		}
	}
}