namespace TalentStream.Core.DTOs.JobPosting
{
    public class UpdateJobDto
    {
		public string? Title { get; set; }
		public string? Description { get; set; }
		public decimal? SalaryRange { get; set; }
	}
}