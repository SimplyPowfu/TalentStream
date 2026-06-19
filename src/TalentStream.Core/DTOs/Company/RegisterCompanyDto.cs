using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.Company
{
	public class RegisterCompanyDto
	{
		[Required(ErrorMessage = "Il nome dell'azienda è obbligatorio.")]
		public string Name { get; set; } = string.Empty;
		[Required(ErrorMessage = "Il settore dell'azienda è obbligatorio.")]
		public string Industry { get; set; } = string.Empty;
		public string? Location { get; set; } = string.Empty;
	}
}