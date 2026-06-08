namespace TalentStream.Core.DTOs.JobPosting
{
	public class RegisterJobDto
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal SalaryRange { get; set; } = default;
	}
}