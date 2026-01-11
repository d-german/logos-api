# Suggested Commands: Logos API (Windows)

## Building
```powershell
# Restore dependencies
dotnet restore

# Build solution
dotnet build logos-api.sln

# Build in Release mode
dotnet build logos-api.sln -c Release

# Clean build artifacts
dotnet clean
```

## Testing
```powershell
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test LogosAPI.Tests/LogosAPI.Tests.csproj

# Run tests in watch mode
dotnet watch test --project LogosAPI.Tests/LogosAPI.Tests.csproj
```

## Running Locally
```powershell
# Run the API (development mode)
dotnet run --project LogosAPI/LogosAPI.csproj

# Run in watch mode (auto-restart on changes)
dotnet watch run --project LogosAPI/LogosAPI.csproj

# Run with specific environment
$env:ASPNETCORE_ENVIRONMENT="Production"; dotnet run --project LogosAPI/LogosAPI.csproj
```

## API Testing
```powershell
# Test endpoints using included PowerShell script
.\test-verses-endpoint.ps1

# Manual testing with Swagger UI (when running locally)
# Navigate to: http://localhost:5133/swagger
```

## Docker Commands
```powershell
# Build Docker image
docker build -t logos-api .

# Run Docker container locally
docker run -p 8000:8000 logos-api

# Test Docker container health
curl http://localhost:8000/health
```

## Code Quality
```powershell
# Format code (if .editorconfig is added)
dotnet format

# List outdated packages
dotnet list package --outdated

# Update packages (be careful with breaking changes)
dotnet add package <PackageName>
```

## Git Commands (Windows)
```powershell
# Status
git status

# Stage changes
git add .

# Commit
git commit -m "Your message"

# Push
git push

# Pull latest
git pull

# View log
git log --oneline
```

## File System (Windows PowerShell)
```powershell
# List directory contents
Get-ChildItem
# or use alias: ls, dir

# List with details
Get-ChildItem -Force

# Find files
Get-ChildItem -Recurse -Filter "*.cs"

# Search file content (grep equivalent)
Select-String -Path "*.cs" -Pattern "searchterm"

# Change directory
Set-Location path\to\directory
# or use alias: cd

# Current directory
Get-Location
# or use: pwd

# Create directory
New-Item -ItemType Directory -Path "NewFolder"

# Remove file/directory
Remove-Item path\to\item
# or with force: Remove-Item -Recurse -Force
```

## Project-Specific Notes
- **Default Port**: Local development runs on http://localhost:5133
- **Swagger UI**: Always enabled (including production) at `/swagger`
- **Health Endpoints**: `/health` and `/_health`
- **Privacy Policy**: Available at `/privacy`
- **Embedded Resources**: Data files are compiled into DLL; changes require rebuild
