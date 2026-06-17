using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.Candidate
{
    public class WorkExperienceDto
    {
        [Required(ErrorMessage = "Il nome dell'azienda è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il nome dell'azienda non può superare i 100 caratteri.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il ruolo svolto è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il ruolo non può superare i 100 caratteri.")]
        public string Role { get; set; } = string.Empty;

		[Required(ErrorMessage = "Gli anni di esperienza svolti è obbligatorio.")]
        public int YearsOfExperience { get; set; }
    }
}