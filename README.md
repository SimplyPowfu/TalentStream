# Struttura
```
TalentStream/
│
├── .editorconfig               # Regole di formattazione e stile di codice standard per .NET
├── .env
├── .gitignore
├── docker-compose.yml
├── Dockerfile
├── Makefile                    # Automazione dei comandi (Make up, down, update, database, migrations)
├── TalentStream.sln
│
├── src/
│   ├── TalentStream.Core/             # LAYER LOGICA DI DOMINIO (Puro, indipendente da framework e DB)
│   │   ├── TalentStream.Core.csproj   # Configurazione del progetto Core (.NET 6.0)
│   │   ├── DTOs/                      # Data Transfer Objects per validare l'input/output delle API
│   │   │   ├── Candidate/             # DTO per registrazione e aggiornamento del profilo del Candidato
│   │   │   ├── Company/               # DTO per l'anagrafica aziendale
│   │   │   ├── JobPosting/            # DTO per la pubblicazione e visualizzazione degli annunci
│   │   │   ├── User/                  # DTO per i flussi di autenticazione (Login, Registrazione, Update)
│   │   │   └── jobApplication/        # DTO per la gestione dello stato della candidatura
│   │   │
│   │   ├── Entities/                  # Modelli e tabelle di business puri (Classi C#)
│   │   │   ├── ApplicationUser.cs     # Modello Utente (Anagrafica, Ruoli "Candidate"/"Recruiter") -> Destinato a SQL
│   │   │   ├── CandidateProfile.cs    # CV, competenze fluide ed esperienze -> Destinato a MongoDB
│   │   │   ├── Company.cs             # Modello Azienda Cliente (Relazione 1-a-Molti con annunci) -> Destinato a SQL
│   │   │   ├── JobApplication.cs      # Tabella pivot d'intersezione Molti-a-Molti per le candidature -> Destinato a SQL
│   │   │   └── JobPosting.cs          # Annuncio di lavoro associato a una Company -> Destinato a SQL
│   │   │
│   │   └── Repositories/              # Contratti (Interfacce) che definiscono i metodi di accesso ai dati
│   │       ├── IApplicationUser.cs    # Contratto per le operazioni sugli utenti
│   │       ├── ICandidate.cs          # Contratto per l'accesso al profilo fluido su MongoDB
│   │       ├── ICompany.cs            # Contratto per la gestione delle aziende
│   │       ├── IJobApplicationReposiroty.cs # Contratto per l'invio e cambio stato delle candidature
│   │       └── IJobPosting.cs         # Contratto per la gestione e paginazione degli annunci
│   │
│   ├── TalentStream.Infrastructure/   # LAYER IMPLEMENTAZIONE TECNOLOGICA (EF Core, Driver Mongo)
│   │   ├── TalentStream.Infrastructure.csproj # Dipendenze esterne (Microsoft.EntityFrameworkCore, MongoDB.Driver)
│   │   ├── Persistence/               # Configurazione fisica e contesti di accesso ai database
│   │   │   ├── MongoDbContext.cs      # Configurazione del driver nativo MongoDB e Fluent Mappings (BsonClassMap)
│   │   │   └── SqlDbContext.cs        # Contesto EF Core per SQL Server (Configurazione Decimal precision)
│   │   │
│   │   └── Repositories/              # Implementazione concreta delle interfacce definite nel Core
│   │       ├── Candidate.cs           # Operazioni CRUD asincrone su MongoDB Collection
│   │       ├── CompanyRepository.cs   # Gestione Aziende tramite Entity Framework Core
│   │       ├── JobApplicationRepository.cs # Gestione transazioni ed Include della tabella pivot SQL
│   │       ├── JobRepository.cs       # Implementazione della query di paginazione ottimizzata con Skip/Take
│   │       └── UserRepository.cs      # Gestione persistenza e lookup degli utenti su SQL Server
│   │
│   ├── TalentStream.WebApi/           # LAYER ESPOSIZIONE (Punto di ingresso REST API ASP.NET Core)
│   │   ├── TalentStream.WebApi.csproj # Dipendenze web (Swashbuckle/Swagger, Authentication.JwtBearer, BCrypt)
│   │   ├── appsettings.json           # Configurazioni locali standard e fallback di sviluppo
│   │   ├── Program.cs                 # Configurazione della Pipeline HTTP, Middlewares, Filtri e Dependency Injection
│   │   │
│   │   ├── Controllers/               # Controller REST che espongono gli endpoint dell'applicazione
│   │   │   ├── AuthController.cs      # Registrazione, Login, hashing BCrypt, emissione e revoca token JWT
│   │   │   ├── CandidateController.cs # Gestione profilo MongoDB e Upload/Download sicuro dei CV in PDF
│   │   │   ├── CompanyController.cs   # Gestione delle informazioni e dei dipendenti dell'azienda
│   │   │   ├── JobApplicationController.cs # Flusso candidature (Apply, cambio stato, cancellazione)
│   │   │   ├── JobController.cs       # Endpoint pubblici e privati per la ricerca paginata di lavoro
│   │   │
│   │   └── Filter/                    # Filtri di azione asincroni (IAsyncActionFilter) per la sicurezza e RBAC
│   │       ├── AuthorizeCandidateAttribute.cs # Valida il token e inietta nel contesto il profilo MongoDB del candidato
│   │       ├── AuthorizeJobOwnerAttribute.cs  # Sicurezza BOLA: Verifica che l'annuncio appartenga all'azienda del recruiter
│   │       └── AuthorizeUserAttribute.cs      # Estrae l'utente da SQL Server basandosi sulle informazioni del JWT
│   │
│   └── TalentStream.Frontend/         # INTERFACCIA UTENTE (Sito Web SPA)
│       ├── Dockerfile                 # Istruzioni Docker per servire l'app Web su server Nginx/Node
│       ├── index.html                 # Pagina HTML principale di ingresso dell'applicazione
│       ├── package.json               # Dipendenze frontend e script npm (React, Redux, Tailwind)
│       └── src/                       # Codice sorgente dell'interfaccia React
│           ├── main.js                # Inizializzatore globale dell'applicazione client
│           ├── api.js                 # Centralizzazione dei client HTTP/Fetch diretti verso il Backend (Porta 5000)
│           ├── components/            # Blocchi grafici riutilizzabili (Navbar, Layout, ProtectedRoute)
│           └── pages/                 # Pagine principali (Login, CandidateDashboard, RecruiterDashboard)
│
└── uploads/                           # Volume condiviso per lo storage fisico locale dei CV (cvs/*.pdf)
```

# Configurazione .env
```
# Configurazione Database SQL Server
SQL_PASSWORD=Password123!
SQL_CONNECTION_STRING=Server=sql_server_db,1433;Database=TalentStreamDb;User Id=sa;Password=Password123!;TrustServerCertificate=True;

# Configurazione MongoDB (Host combinato con il nome servizio del compose)
MONGO_CONNECTION_STRING=mongodb://root:pass@mongodb:27017/TalentStreamMongoDb?authSource=admin

# Configurazione Sicurezza JWT
JWT_SECRET=Secret_Talent_Stream_123456_Bisogna_Allungare_La_Stringa_Per_I_256_Bit
JWT_EXPIRY=60
JWT_ISSUER=TalentStreamBackend
JWT_AUDIENCE=TalentStreamFrontend
```