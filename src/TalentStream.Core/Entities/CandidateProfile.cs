namespace TalentStream.Core.Entities
{
	public class CandidateProfile
	{
		// Mongo genera automaticamente degli id univoci di 24 caratteri alfanumerici chiamati ObjectId.
		public string Id { get; set; } = string.Empty;

		public string JobPosition { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string Number { get; set; } = string.Empty;

		// Una lista di stringhe per le competenze chiave (es: ["C#", "SQL", "Docker", "React"])
		public List<string>? Skills { get; set; } = new();

		// Una lista di oggetti interni per lo storico lavorativo.
		public List<WorkExperience> Experiences { get; set; } = new();

		public string? CvUrl { get; set; } = string.Empty;

		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		public int UserId { get; set; }
	}

	// Classe di supporto che descrive la singola esperienza lavorativa.
	public class WorkExperience
	{
		public string CompanyName { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public int YearsOfExperience { get; set; }
	}
}