using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TalentStream.Core.Entities;

namespace TalentStream.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // Il costruttore legge la stringa di connessione e il nome del DB direttamente da appsettings.json
        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);
            
            var databaseName = configuration["MongoSettings:DatabaseName"];
            _database = client.GetDatabase(databaseName);
        }

        // Espone la collezione dei profili dei candidati.
        // Se la collezione "Candidates" non esiste ancora nel DB, MongoDB la creerà automaticamente al primo inserimento.
        public IMongoCollection<CandidateProfile> Candidates => 
            _database.GetCollection<CandidateProfile>("Candidates");
    }
}