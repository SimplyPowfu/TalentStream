using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentStream.Core.Entities;
using TalentStream.Core.Repositories;
using TalentStream.Core.DTOs.Candidate;
using TalentStream.WebApi.Filters;
using TalentStream.WebApi.Filter;
using System.Globalization;
namespace TalentStream.WebApi.Controllers
{
	public class UploadCv
	{
		public IFormFile Cv { get; set; } = null!;
	}

	[ApiController]
	[Route("api/[controller]")]
	public class CandidateController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly ICandidateRepository _candidateRepository;

		public CandidateController(IUserRepository userRepository, ICandidateRepository candidateRepository)
		{
			_userRepository = userRepository;
			_candidateRepository = candidateRepository;
		}

		[HttpPost("register")]
		[Authorize(Roles = "Candidate")]
		public async Task<IActionResult> Register(RegisterCandidateDto dto)
		{
			var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userIdClaim))
				return Unauthorized(new { message = "Utente non identificato nel token." });
			int userId = int.Parse(userIdClaim);

			var existingProfile = await _candidateRepository.GetByUserIdAsync(userId);
			if (existingProfile != null)
			{
				return BadRequest(new { message = "Hai già creato un profilo. Se vuoi modificarlo, usa l'endpoint di aggiornamento." });
			}

			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
				return NotFound(new { message = "Utente non trovato." });

			var newCandidate = new CandidateProfile
			{
				JobPosition = dto.JobPosition,
				Email = user.Email,
				Number = dto.Number,
				Skills = dto.Skills ?? new List<string>(),
				Experiences = (dto.Experiences ?? new List<WorkExperienceDto>())
					.Select(exp => new WorkExperience
					{
						CompanyName = exp.CompanyName,
						Role = exp.Role,
						YearsOfExperience = exp.YearsOfExperience
					}).ToList(),
				UserId = user.Id
			};

			await _candidateRepository.AddAsync(newCandidate);
			return CreatedAtAction(nameof(Register), new { message = "Profilo salvato con successo." });
		}

		[HttpGet("profile")]
		[Authorize(Roles = "Candidate")]
		[AuthorizeCandidate]
		public ActionResult<CandidateProfile> GetProfile()
		{
			var profile = HttpContext.Items["ValidatedProfile"] as CandidateProfile;
			if (profile == null)
				return StatusCode(500, new { message = "[GET] Errore nel recupero del profilo" });
			return Ok(new { message = "Profilo Recuperato con successo", profile });
		}

		[HttpGet("profile/{id}")]
		[Authorize(Roles = "Recruiter")]
		public ActionResult<CandidateProfile> GetProfile(int id)
		{
			var profile = _candidateRepository.GetByUserIdAsync(id);
			if (profile.Result == null)
				return NotFound(new { message = "Profilo non trovato" });
			return Ok(new { message = "Profilo Recuperato con successo", profile = profile.Result });
		}


		[HttpPatch("profile")]
		[Authorize(Roles = "Candidate")]
		[AuthorizeCandidate]
		public async Task<IActionResult> UpdateProfile(UpdateCandidateDto dto)
		{
			var profile = HttpContext.Items["ValidatedProfile"] as CandidateProfile;
			if (profile == null)
				return StatusCode(500, new { message = "[UPDATE] Errore nel recupero del profilo" });

			profile.JobPosition = dto.JobPosition ?? profile.JobPosition;
			profile.Email = dto.Email ?? profile.Email;
			profile.Number = dto.Number ?? profile.Number;
			if (dto.Skills != null && dto.Skills.Any())
			{
				profile.Skills ??= new List<string>();
				profile.Skills.AddRange(dto.Skills);
				profile.Skills = profile.Skills.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			}
			if (dto.Experiences != null && dto.Experiences.Any())
			{
				var newExperiences = dto.Experiences.Select(exp => new WorkExperience
				{
					CompanyName = exp.CompanyName,
					Role = exp.Role,
					YearsOfExperience = exp.YearsOfExperience
				}).ToList();
				profile.Experiences.AddRange(newExperiences);
			}
			profile.UpdatedAt = DateTime.UtcNow;
			await _candidateRepository.UpdateAsync(profile);
			return Ok(new { message = "Profilo aggiornato." });
		}

		[HttpPost("upload-cv")]
		[Authorize(Roles = "Candidate")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UploadCv([FromForm] UploadCv request)
		{
			var cv = request.Cv;

			if (cv == null || cv.Length == 0)
				return BadRequest(new { message = "[POST] Nessun file selezionato o file vuoto." });
			var extension = Path.GetExtension(cv.FileName).ToLowerInvariant();
			if (extension != ".pdf")
				return BadRequest(new { message = "Formato non supportato. Puoi caricare solo file PDF." });

			var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userIdClaim))
				return Unauthorized(new { message = "Utente non identificato." });
			int userId = int.Parse(userIdClaim);

			var profile = await _candidateRepository.GetByUserIdAsync(userId);
			if (profile == null)
				return NotFound(new { message = "Profilo candidato non trovato. Registrati prima di caricare il CV." });

			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var uniqueFileName = $"cv_{userId}_{DateTime.UtcNow.Ticks}{extension}";
			var filePath = Path.Combine(uploadsFolder, uniqueFileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
				await cv.CopyToAsync(stream);

			if (!string.IsNullOrEmpty(profile.CvUrl))
			{
				var relativePath = profile.CvUrl.TrimStart('/');
				var vecchioFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
				if (System.IO.File.Exists(vecchioFilePath))
					System.IO.File.Delete(vecchioFilePath);
			}
			profile.CvUrl = $"/uploads/cvs/{uniqueFileName}";
			profile.UpdatedAt = DateTime.UtcNow;

			await _candidateRepository.UpdateAsync(profile);
			return Ok(new { message = "CV caricato e collegato al profilo con successo!" });
		}

		[HttpDelete("profile")]
		[Authorize(Roles = "Candidate")]
		[AuthorizeCandidate]
		public async Task<IActionResult> RemoveProfile()
		{
			var user = HttpContext.Items["ProfileUser"] as User;
			if (user == null)
				return StatusCode(500, new { message = "[DELETE] Errore nel recupero del profilo" });
			var profile = HttpContext.Items["ValidatedProfile"] as CandidateProfile;
			if (profile == null)
				return StatusCode(500, new { message = "[DELETE] Errore nel recupero del profilo" });

			if (!string.IsNullOrEmpty(profile.CvUrl))
			{
				var relativePath = profile.CvUrl.TrimStart('/');
				var vecchioFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
				if (System.IO.File.Exists(vecchioFilePath))
					System.IO.File.Delete(vecchioFilePath);
			}

			await _candidateRepository.Delete(user.Id);
			return Ok(new { message = "Profilo Cancellato" });
		}

	}
}