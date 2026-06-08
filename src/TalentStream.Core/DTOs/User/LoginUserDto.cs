using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.User
{
	public class LoginUserDto
	{
		[Required(ErrorMessage = "L'email è obbligatoria.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "La password è obbligatoria.")]
		public string Password { get; set; } = string.Empty;
	}
}