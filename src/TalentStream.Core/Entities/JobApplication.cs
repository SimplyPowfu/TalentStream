namespace TalentStream.Core.Entities
{
	public class JobApplication
	{
		public int Id { get; set; }

		public int JobPostingId { get; set; }
		public JobPosting JobPosting { get; set; } = null!;

		public int UserId { get; set; }
		public User User { get; set; } = null!;

		public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
		public string Status { get; set; } = "Pending"; // Pending, Reviewing, Accepted, Rejected
	}
}