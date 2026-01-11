# Tech Stack: Logos API

## Core Technologies
- **.NET 8.0**: Latest LTS version of .NET (SDK 8.0.0)
- **ASP.NET Core Web API**: RESTful API framework
- **C# 11+**: With nullable reference types enabled
- **Implicit Usings**: Enabled for cleaner code

## Key NuGet Packages

### Production
- **Swashbuckle.AspNetCore 6.6.2**: OpenAPI/Swagger documentation

### Testing
- **xUnit 2.5.3**: Testing framework
- **xunit.runner.visualstudio 2.5.3**: Test runner
- **Moq 4.20.70**: Mocking framework
- **Microsoft.AspNetCore.Mvc.Testing 8.0.0**: Integration testing
- **Microsoft.NET.Test.Sdk 17.8.0**: Test platform
- **coverlet.collector 6.0.0**: Code coverage

## Data Storage
- **Embedded Resources**: All data files (verses.json, lexicon.json, fronting.json, semantic-domains.json, marble-domain-label-mapping.json) are embedded as resources for containerized deployment
- **In-Memory**: ConcurrentDictionary for fast data access (Singleton pattern)
- **No Database**: Stateless API with pre-loaded data

## Architecture Patterns
- **Dependency Injection**: Built-in ASP.NET Core DI container
- **Singleton Services**: All data services registered as singletons for performance
- **Interface-based Design**: All services implement interfaces (e.g., IBibleDataService)
- **Controller-Service Pattern**: Controllers delegate to service layer
- **SOLID Principles**: Strong adherence throughout codebase

## Deployment Stack
- **Docker**: Multi-stage builds (SDK + ASP.NET Runtime)
- **Koyeb**: Cloud hosting platform
- **Port 8000**: Production API port
- **Memory Optimization**: GC heap hard limit (953MB) for 1GB container
