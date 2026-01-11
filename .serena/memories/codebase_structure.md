# Codebase Structure: Logos API

## Solution Structure
```
logos-api/
├── LogosAPI/                    # Main API project
│   ├── Controllers/             # API endpoints
│   ├── Services/                # Business logic layer
│   ├── Models/                  # Data models/DTOs
│   ├── Data/                    # JSON data files (embedded resources)
│   ├── Properties/              # Launch settings
│   ├── Program.cs               # Application entry point & DI configuration
│   ├── appsettings.json         # Configuration
│   └── LogosAPI.csproj          # Project file
├── LogosAPI.Tests/              # Test project
│   ├── *Tests.cs                # Unit and integration tests
│   └── LogosAPI.Tests.csproj    # Test project file
├── logos-api.sln                # Solution file
├── Dockerfile                   # Container build definition
└── test-verses-endpoint.ps1     # API testing script
```

## Controllers Layer
**Location**: `LogosAPI/Controllers/`

- `VersesController.cs` - Verse lookup endpoints (GET/POST `/api/verses/lookup`)
- `LexiconController.cs` - Strong's number lookups
- `SemanticDomainController.cs` - Louw-Nida semantic domain queries
- `CommentaryController.cs` - External commentary integration

**Pattern**: Controllers are thin, delegating to services. Use DI to inject service dependencies.

## Services Layer
**Location**: `LogosAPI/Services/`

### Core Data Services
- `BibleDataService.cs` / `IBibleDataService.cs` - Loads and provides verses + lexicon data
- `VerseLookupService.cs` / `IVerseLookupService.cs` - Orchestrates verse lookup operations
- `FrontingDataService.cs` / `IFrontingDataService.cs` - Word order/discourse feature data
- `SemanticDomainService.cs` / `ISemanticDomainService.cs` - Louw-Nida domain data

### Parsing/Normalization Services
- `VerseReferenceNormalizer.cs` / `IVerseReferenceNormalizer.cs` - Parses verse references (e.g., "Matt.1.1")
- `StrongsNumberNormalizer.cs` / `IStrongsNumberNormalizer.cs` - Normalizes Strong's numbers (e.g., "G25")
- `RmacParser.cs` / `IRmacParser.cs` - Parses Greek morphology codes

### External Integration
- `CommentaryService.cs` / `ICommentaryService.cs` - Calls HelloAO API for commentary
- `WordFrequencyService.cs` / `IWordFrequencyService.cs` - Calculates word frequency data
- `LouwNidaService.cs` / `ILouwNidaService.cs` - Maps Strong's to Louw-Nida domains

**Pattern**: All services implement interfaces. Most are singletons. Use functional approach where possible.

## Models Layer
**Location**: `LogosAPI/Models/`

### Request/Response DTOs
- `VerseLookupResponse.cs` - Verse lookup API response
- `LexiconLookupResponse.cs` - Lexicon API response
- `TokenResponse.cs` - Token data in API responses
- `VerseResponse.cs` - Individual verse in response

### Domain Models
- `VerseData.cs` - Internal verse data structure
- `TokenData.cs` - Word token with morphology/lexicon
- `StrongsInfo.cs` - Strong's number with definition/frequency
- `MorphologyInfo.cs` - Parsed morphology data
- `FrontingInfo.cs` - Discourse fronting information
- `DomainEntry.cs` - Louw-Nida semantic domain entry
- `RelatedWordsResult.cs` - Semantic domain related words

**Pattern**: Use `sealed class`, `required` properties, `init` accessors for immutability.

## Data Layer
**Location**: `LogosAPI/Data/`

- `verses.json` - Full Greek NT with tokenization, morphology, Strong's numbers
- `lexicon.json` - Strong's concordance definitions
- `fronting.json` - Clause-level discourse features
- `semantic-domains.json` - Louw-Nida domain classifications
- `marble-domain-label-mapping.json` - Domain label mappings

**Pattern**: All files are embedded as resources via `.csproj`. Loaded at startup into ConcurrentDictionary.

## Tests Layer
**Location**: `LogosAPI.Tests/`

- `VersesControllerIntegrationTests.cs` - Full API endpoint tests
- `VerseLookupServiceTests.cs` - Service orchestration tests
- `VerseReferenceNormalizerTests.cs` - Parsing logic tests
- `StrongsNumberNormalizerTests.cs` - Normalization tests
- `RmacParserTests.cs` - Morphology parsing tests
- `FrontingDataServiceTests.cs` - Discourse data tests
- `SemanticDomainServiceTests.cs` - Semantic domain tests
- `DataIntegrityTests.cs` - Embedded resource validation

**Pattern**: xUnit framework. Use `WebApplicationFactory` for integration tests.

## Configuration Files
- `Program.cs` - DI container setup, middleware pipeline, endpoint mapping
- `appsettings.json` - Application configuration
- `appsettings.Development.json` - Dev-specific overrides
- `global.json` - .NET SDK version pinning
- `Dockerfile` - Multi-stage Docker build

## Key Architectural Decisions
1. **No Database**: All data pre-loaded from embedded JSON
2. **Singleton Pattern**: Services load data once, shared across requests
3. **Thread-Safe**: ConcurrentDictionary for concurrent access
4. **Stateless**: No user sessions or persistent state
5. **Containerized**: Optimized for Docker/Koyeb deployment with memory limits
