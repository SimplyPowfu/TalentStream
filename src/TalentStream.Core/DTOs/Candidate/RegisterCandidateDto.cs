using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.Candidate
{
    public class RegisterCandidateDto
    {
		[Required(ErrorMessage = "La posizione lavorativa e' obbligatoria.")]
		public string JobPosition { get; set; } = string.Empty;

		[Required(ErrorMessage = "Il numero e' obbligatoria.")]
		[Phone(ErrorMessage = "Il formato del numero di telefono e' invalido.")]
		public string Number { get; set; } = string.Empty;

		public List<string>? Skills { get; set; } = new();

		public List<WorkExperienceDto>? Experiences { get; set; } = new();
    }
}