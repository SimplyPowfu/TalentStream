using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.Company
{
	public class RegisterCompanyDto
	{
		[Required(ErrorMessage = "Il nome dell'azienda è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri.")]
		public string Name { get; set; } = string.Empty;
		public string Industry { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
	}
}