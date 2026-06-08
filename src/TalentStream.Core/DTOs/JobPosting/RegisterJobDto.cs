using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.JobPosting
{
	public class RegisterJobDto
	{
		[Required(ErrorMessage = "Il titolo della posizione è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il titolo non può superare i 100 caratteri.")]
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal SalaryRange { get; set; } = default;
	}
}