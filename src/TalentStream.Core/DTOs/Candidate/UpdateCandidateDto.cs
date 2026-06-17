using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.Candidate
{
    public class UpdateCandidateDto
    {
        public string? JobPosition { get; set; }

		[EmailAddress(ErrorMessage = "Il formato dell'email non è valido.")]
		public string? Email { get; set; }

		[Phone(ErrorMessage = "Il formato del numero di telefono e' invalido.")]
		public string? Number { get; set; }

		public List<string>? Skills { get; set; } = new();

		public List<WorkExperienceDto>? Experiences { get; set; } = new();
    }
}