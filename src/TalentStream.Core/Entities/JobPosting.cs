namespace TalentStream.Core.Entities
{
    public class JobPosting
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty; // Es: Full Stack Developer
        
        public string Description { get; set; } = string.Empty;
        
        public decimal? SalaryRange { get; set; } // Budget massimo per la posizione
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Chiave Esterna (Foreign Key): Collega l'annuncio alla specifica azienda che lo ha creato.
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}