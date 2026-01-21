# Logos API

A RESTful Bible verse lookup API with comprehensive Greek/Hebrew lexical data, morphology, semantic domains, and linguistic analysis.

## Features

- **Verse Lookup**: Full tokenization with Greek morphology (RMAC codes)
- **Lexicon Data**: Strong's concordance numbers with definitions
- **Semantic Domains**: Louw-Nida semantic domain classifications
- **Word Frequency**: Hapax legomena detection and frequency analysis
- **Discourse Features**: Clause fronting and word order analysis
- **Commentary Integration**: External commentary data via HelloAO API

## Tech Stack

- **.NET 8.0** with ASP.NET Core Web API
- **Embedded Resources**: All data files compiled into DLL (no database)
- **Docker**: Multi-stage builds for containerized deployment
- **Koyeb**: Cloud hosting platform

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Installation

1. **Clone the repository**
   ```powershell
   git clone https://github.com/YOUR_USERNAME/logos-api.git
   cd logos-api
   ```

2. **Restore dependencies**
   ```powershell
   dotnet restore
   ```

3. **Build the solution**
   ```powershell
   dotnet build logos-api.sln
   ```

4. **Run tests**
   ```powershell
   dotnet test
   ```

5. **Run the API locally**
   ```powershell
   dotnet run --project LogosAPI/LogosAPI.csproj
   ```

   The API will be available at `http://localhost:5133`
   Swagger UI: `http://localhost:5133/swagger`

### MCP Server Configuration (Optional)

If you're using MCP (Model Context Protocol) servers for AI-assisted development:

1. **Copy the template configuration**
   ```powershell
   Copy-Item .vscode\mcp.json.template .vscode\mcp.json
   ```

2. **Edit `.vscode/mcp.json`** and update paths:
   - Replace `PATH_TO_YOUR_MCP_SERVER` with your actual MCP server DLL path
   - Replace `YOUR_USERNAME` with your Windows username
   - The `task-and-research` server path should point to where you published the MCP server DLL

3. **Publishing the task-and-research MCP server** (if needed):
   - Navigate to the task-and-research project directory
   - Run: `dotnet publish -c Release -o C:\Users\YOUR_USERNAME\.mcp-servers\task-and-research`
   - Update the path in your `mcp.json` accordingly

**Note**: The `.vscode/mcp.json` file is gitignored as it contains user-specific paths.

## Development

### Project Structure

```
logos-api/
├── LogosAPI/              # Main API project
│   ├── Controllers/       # API endpoints
│   ├── Services/          # Business logic
│   ├── Models/            # Data models
│   └── Data/              # JSON data files (embedded)
├── LogosAPI.Tests/        # Test project
└── Dockerfile             # Container build
```

### Running Tests

```powershell
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### API Endpoints

- `GET /api/verses/lookup` - Lookup verses by reference
- `POST /api/verses/lookup` - Lookup multiple verses
- `GET /api/lexicon/{strongsNumber}` - Get Strong's definition
- `GET /api/commentary/{commentaryId}/{reference}` - Get specific commentary
- `GET /api/commentary/all` - Get all commentaries for verses
- `GET /api/semantic-domain/related` - Get related words by semantic domain
- `GET /health` - Health check endpoint

### Docker

```powershell
# Build Docker image
docker build -t logos-api .

# Run container locally
docker run -p 8000:8000 logos-api

# Test
curl http://localhost:8000/health
```

## Code Style

This project follows strict **SOLID principles** and emphasizes:

- **Functional Programming**: Immutability, pure functions, higher-order functions
- **Static Methods**: Methods without instance state must be static
- **Nullable Reference Types**: Enabled project-wide
- **XML Documentation**: Required for all public APIs
- **Singleton Services**: All services are singletons for performance

See `.editorconfig` for detailed style rules (if available).

## Contributing

1. Create a feature branch
2. Make your changes
3. Ensure all tests pass: `dotnet test`
4. Ensure build succeeds: `dotnet build`
5. Submit a pull request

## API Documentation

Full API documentation is available via Swagger UI when running the application:
- Local: `http://localhost:5133/swagger`
- Production: `https://your-deployment-url/swagger`

## License

[Specify your license here]

## Privacy

See `/privacy` endpoint for the privacy policy.
