# LogosAPI Project Overview

**Date:** January 1, 2026  
**Framework:** ASP.NET Core 8.0 Web API  
**Deployment Target:** Koyeb (PaaS)

---

## Project Purpose

LogosAPI is a Web API for serving biblical/lexicon data. It includes:
- Static JSON data files (lexicon.json, verses.json)
- RESTful API endpoints
- Swagger/OpenAPI documentation

---

## Project Structure

```
C:\projects\github\logos-api\
├── LogosAPI/                    # Main API project
│   ├── Controllers/             # API controllers
│   │   └── WeatherForecastController.cs
│   ├── Data/                    # Static data files
│   │   ├── lexicon.json
│   │   └── verses.json
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── LogosAPI.csproj
│   ├── Program.cs
│   └── WeatherForecast.cs
├── LogosAPI.slnx                # Solution file
├── global.json                  # SDK version
└── .gitignore
```

---

## Key Files

### LogosAPI.csproj
- Target: net8.0
- Dependencies: Swashbuckle.AspNetCore (Swagger)

### Program.cs
- Standard ASP.NET Core Web API setup
- Swagger enabled in Development mode
- Controllers mapped via `app.MapControllers()`

### Data Files
- `lexicon.json` - Lexicon/dictionary data
- `verses.json` - Biblical verse data
- **Important:** These must be included in publish output for deployment

---

## Local Development

### Run API
```bash
cd LogosAPI
dotnet run
```

### URLs
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger:** http://localhost:5000/swagger

---

## Configuration

### appsettings.json
Standard ASP.NET Core configuration. Environment-specific settings in:
- `appsettings.Development.json`
- `appsettings.Production.json` (create as needed)

### Environment Variables
- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ASPNETCORE_URLS` - Binding URLs

---

## Git Workflow

### Branches
- `main` - Production branch (triggers Koyeb deploy)

### Commands
```bash
git status              # Check changes
git add -A              # Stage all
git commit -m "msg"     # Commit
git push origin main    # Deploy to Koyeb
```

---

## Related Memories

- `koyeb-deployment-guide.md` - Deployment instructions
- `docker-configuration.md` - Docker setup (to be created)
- `troubleshooting.md` - Common issues (to be created)

---

## TODO

- [ ] Create Dockerfile for Koyeb deployment
- [ ] Configure Data files to be included in publish
- [ ] Add health endpoint
- [ ] Create API controllers for lexicon/verses data
- [ ] Test deployment on Koyeb
