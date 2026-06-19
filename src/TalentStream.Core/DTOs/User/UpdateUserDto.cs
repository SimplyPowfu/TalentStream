using System.ComponentModel.DataAnnotations;

namespace TalentStream.Core.DTOs.User
{
    public class UpdateUserDto
    {
		public string? Name { get; set; }

		public string? Surname { get; set; }

        [EmailAddress(ErrorMessage = "Il formato dell'email non è valido.")]
		public string? Email { get; set; }

        [MinLength(8, ErrorMessage = "La password deve essere lunga almeno 8 caratteri.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&])[A-Za-z@$!%*?&].{7,}$", 
    	ErrorMessage = "La password deve contenere almeno una lettera maiuscola, una minuscola e un carattere speciale (@$!%*?&).")]
		public string? Password { get; set; }
    }
}