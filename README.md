```
TalentStream/
│
├── docker-compose.yml                 # Configurazione dei container (SQL, Mongo, Redis, API)
├── TalentStream.sln            # File di Soluzione globale .NET
│
├── src/
│   ├── TalentStream.Core/             # LOGICA DI BUSINESS (Nessuna dipendenza esterna)
│   │   ├── TalentStream.Core.csproj
│   │   ├── Entities/                  # Modelli di dominio puri (Classi C#)
│   │   │   ├── User.cs     # Dati utente e credenziali
│   │   │   ├── Company.cs             # Anagrafica aziende clienti Experis
│   │   │   ├── JobPosting.cs          # Annunci di lavoro (destinati a SQL)
│   │   │   └── CandidateProfile.cs    # CV e competenze fluide (destinato a Mongo)
│   │   │
│   │   ├── Interfaces/                # Contratti per disaccoppiare l'infrastruttura
│   │   │   ├── ISqlRepository.cs
│   │   │   ├── IMongoRepository.cs
│   │   │   └── ICacheService.cs
│   │   │
│   │   └── Services/                  # Algoritmi e logica di calcolo
│   │       └── MatchingEngine.cs      # Algoritmo C# per calcolare il punteggio di compatibilità
│   │
│   ├── TalentStream.Infrastructure/   # IMPLEMENTAZIONE TECNOLOGICA (EF Core, Mongo, Redis)
│   │   ├── TalentStream.Infrastructure.csproj
│   │   ├── Persistence/
│   │   │   ├── SqlDbContext.cs        # Context di Entity Framework Core 6.0 per SQL Server
│   │   │   ├── MongoContext.cs        # Configurazione del driver nativo MongoDB
│   │   │   └── Migrations/            # Storico delle migrazioni SQL autogenerate
│   │   │
│   │   ├── Repositories/              # Implementazione concreta delle interfacce del Core
│   │   │   ├── SqlRepository.cs
│   │   │   └── MongoRepository.cs
│   │   │
│   │   └── Cache/
│   │       └── RedisCacheService.cs   # Implementazione di StackExchange.Redis
│   │
│   ├── TalentStream.WebApi/           # STRATO DI ESPOSIZIONE (ASP.NET Core Web API)
│   │   ├── TalentStream.WebApi.csproj
│   │   ├── Program.cs                 # Configurazione Dependency Injection, Pipeline e Middleware
│   │   ├── appsettings.json           # Stringhe di connessione locali e JWT settings
│   │   ├── Dockerfile                 # Istruzioni Docker per containerizzare l'API .NET
│   │   │
│   │   ├── Controllers/               # Endpoint REST
│   │   │   ├── AuthController.cs      # Registrazione, Login e generazione JWT
│   │   │   ├── JobPostingsController.cs
│   │   │   ├── CandidatesController.cs
│   │   │   └── MatchingController.cs   # Endpoint che espone i risultati dell'algoritmo
│   │   │
│   │   └── Middlewares/
│   │       └── ExceptionHandlingMiddleware.cs # Gestore globale degli errori API
│   │
│   └── talentstream-frontend/         # INTERFACCIA UTENTE (Sito Web SPA)
│       ├── package.json
│       ├── index.html
│       ├── src/
│       │   ├── main.js                # Punto di ingresso JavaScript
│       │   ├── styles.css             # Configurazione Tailwind CSS
│       │   ├── api.js                 # Centralizzazione delle chiamate fetch verso il backend
│       │   │
│       │   ├── components/            # Componenti UI riutilizzabili
│       │   │   ├── Navbar.js
│       │   │   └── ProtectedRoute.js  # Controllo degli accessi in base al ruolo (Recruiter/Candidato)
│       │   │
│       │   └── pages/                 # Schermate principali dell'applicazione
│       │       ├── Login.js           # Form di autenticazione (memorizza il JWT)
│       │       ├── CandidateDashboard.js # Form per aggiornare le proprie skill su MongoDB
│       │       └── RecruiterDashboard.js # Visualizza annunci (SQL) e i match in tempo reale
│       └── public/
│
└── tests/                             # STRATO DI TESTING (Opzionale, ma raccomandato)
    └── TalentStream.UnitTests/
        └── MatchingEngineTests.cs     # Unit test per validare l'algoritmo di matching
```