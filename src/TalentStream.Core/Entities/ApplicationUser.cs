namespace TalentStream.Core.Entities
{
	public class User
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Surame { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string PasswordHash { get; set; } = string.Empty;

		// Es: "Recruiter" (vede i match, crea lavori) o "Candidate" (carica il CV).
		public string Role { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public int? CompanyId { get; set; }
		public Company? Company { get; set; }
	}
}