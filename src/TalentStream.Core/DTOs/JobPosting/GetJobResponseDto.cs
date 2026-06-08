namespace TalentStream.Core.DTOs.JobPosting
{
	public class GetJobResponseDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal? SalaryRange { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; } = string.Empty;
		public string CompanyLocation { get; set; } = string.Empty;
	}
}