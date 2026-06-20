using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.JobPosting;
using TalentStream.WebApi.Filters;
using TalentStream.WebApi.Filter;
using TalentStream.Core.DTOs.jobApplication;

namespace TalentStream.WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JobApplicationController : ControllerBase
	{
		private readonly IJobRepository _jobRepository;
		private readonly IJobApplicationRepository _applicationRepository;
		public JobApplicationController(IJobRepository jobRepository, IJobApplicationRepository applicationRepository)
		{
			_jobRepository = jobRepository;
			_applicationRepository = applicationRepository;
		}

		[HttpPost("apply/{jobPostingId}")]
		[Authorize(Roles = "Candidate")]
		[AuthorizeCandidate]
		public async Task<ActionResult> ApplyToJob(int jobPostingId)
		{
			var user = HttpContext.Items["ProfileUser"] as User;
			if (user == null)
				return NotFound(new { message = "[PATCH] Utente non trovato." });

			var jobPosting = await _jobRepository.GetByIdAsync(jobPostingId);
			if (jobPosting == null)
				return NotFound(new { message = "L'annuncio di lavoro specificato non esiste." });

			bool alreadyApplied = await _applicationRepository.ExistsAsync(user.Id, jobPostingId);
			if (alreadyApplied)
				return BadRequest(new { message = "Ti sei già candidato a questo annuncio di lavoro." });

			var newApplication = new JobApplication
			{
				JobPostingId = jobPostingId,
				UserId = user.Id,
				AppliedAt = DateTime.UtcNow,
				Status = "Pending"
			};

			await _applicationRepository.AddAsync(newApplication);
			await _applicationRepository.SaveChangesAsync();
			return Ok(new { message = "Candidatura inviata con successo!" });
		}

		[HttpGet("my-applications")]
		[Authorize(Roles = "Candidate")]
		[AuthorizeCandidate]
		public async Task<ActionResult> GetMyApllications()
		{
			var user = HttpContext.Items["ProfileUser"] as User;
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });

			var applications = await _applicationRepository.GetByUserIdAsync(user.Id);
			if (applications == null || !applications.Any())
				return NotFound(new { message = "Nessuna JobApplication trovata" });
			var response = applications.Select(a => new
			{
				applicationId = a.Id,
				status = a.Status,
				appliedAt = a.AppliedAt,
				job = new
				{
					id = a.JobPosting.Id,
					title = a.JobPosting.Title,
					companyName = a.JobPosting.Company?.Name,
					location = a.JobPosting.Company?.Location
				}
			});
			return Ok(new { message = "JobApplication recuperate con successo", response });
		}

		[HttpGet("job/{jobPostingId}/applications")]
		[Authorize(Roles = "Recruiter")]
		[AuthorizeUser]
		public async Task<ActionResult> GetJobApplicats(int jobPostingId)
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });
			var jobPosting = await _jobRepository.GetByIdAsync(jobPostingId);
			if (jobPosting == null)
				return NotFound(new { message = "Annuncio non trovato." });
			if (user.CompanyId != jobPosting.CompanyId)
				return Forbid();
			var applications = await _applicationRepository.GetByJobPostingIdAsync(jobPostingId);
			if (applications == null || !applications.Any())
				return NotFound( new { message = "Nessuna candidatura per questa posizione"});
			var response = applications.Select(a => new
			{
				applicationId = a.Id,
				status = a.Status,
				appliedAt = a.AppliedAt,
				user = new
				{
					id = a.User.Id,
					name = a.User.Name,
					surname = a.User.Surname,
					email = a.User.Email
				}
			});
			return Ok(new { message = "Job applicant recuperata con successo.", response });
		}

		[HttpPatch("application/{applicationId}/status")]
		[Authorize(Roles = "Recruiter")]
		[AuthorizeUser]
		public async Task<ActionResult> UpdateJobPosting(int applicationId, UpdateApplicationStatusDto dto)
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });
			var application = await _applicationRepository.GetByIdAsync(applicationId);
			if (application == null)
				return NotFound(new { message = "Candidatura non trovata." });
			if (user.CompanyId != application.JobPosting.CompanyId)
				return Forbid();
			var validStatuses = new[] { "Pending", "Reviewing", "Accepted", "Rejected" };
			if (!validStatuses.Contains(dto.Status))
				return BadRequest(new { message = "Stato non valido. Scegli tra: Pending, Reviewing, Accepted, Rejected." });
			application.Status = dto.Status;
			_applicationRepository.Update(application);
			await _applicationRepository.SaveChangesAsync();
			return Ok(new { message = "Job application aggiornata con successo." });
		}

		[HttpDelete("cancel/{applicationId}/candidate")]
		[Authorize(Roles = "Candidate")]
		[AuthorizeUser]
		public async Task<ActionResult> CancelApplication(int applicationId)
		{
			var user = HttpContext.Items["ValidatedUser"] as User;
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });
			var application = await _applicationRepository.GetByIdAsync(applicationId);
			if (application == null)
				return NotFound(new { message = "Candidatura non trovata." });
			if (application.UserId != user.Id)
				return Forbid();
			_applicationRepository.Delete(application);
			await _applicationRepository.SaveChangesAsync();
			return Ok(new { message = "Candidatura ritirata con successo." });
		}
	}
}