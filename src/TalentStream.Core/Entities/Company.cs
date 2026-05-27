namespace TalentStream.Core.Entities
{
    public class Company
    {
        public int Id { get; set; } //imposta come Chiave Primaria (Primary Key) Auto-Incrementale nel DB.

        public string Name { get; set; } = string.Empty; // Nome dell'azienda

        public string Industry { get; set; } = string.Empty; // Es: Automotive, Banking, IT
        
        public string Location { get; set; } = string.Empty; // Città della sede

        public ICollection<User> Employees { get; set; } = new List<User>();
        public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();//relazione 1 a Molti (One-to-Many) nel database.
    
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}