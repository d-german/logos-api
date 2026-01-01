# Koyeb Deployment Guide for LogosAPI

**Date:** January 1, 2026  
**Project:** LogosAPI - ASP.NET Core 8.0 Web API  
**Target Platform:** Koyeb (PaaS)

---

## Overview

This guide covers deploying an ASP.NET Core Web API to Koyeb, including Docker configuration, persistent storage, and common troubleshooting steps.

---

## Project Structure

```
LogosAPI/
├── Controllers/
│   └── WeatherForecastController.cs
├── Data/
│   ├── lexicon.json          # Static data files
│   └── verses.json           # Static data files
├── LogosAPI.csproj           # Project file (net8.0)
├── Program.cs                # Entry point
└── appsettings.json          # Configuration
```

---

## Dockerfile Template for Koyeb

Create a `Dockerfile` in the repository root:

```dockerfile
# Build stage - Use SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy only the project file first (bypasses .slnx detection, enables layer caching)
COPY LogosAPI/LogosAPI.csproj ./LogosAPI/
RUN dotnet restore LogosAPI/LogosAPI.csproj

# Copy everything else
COPY . .
WORKDIR /source/LogosAPI

# Verify Data files exist in source
RUN echo "=== Checking source Data files ===" && \
    ls -la Data/ && \
    wc -l Data/*.json || echo "No Data files found"

# Publish targeting the .csproj directly (bypasses .slnx)
RUN dotnet publish LogosAPI.csproj -c Release -o /publish --no-restore

# Verify Data files are in publish output
RUN echo "=== Checking publish Data files ===" && \
    ls -la /publish/Data/ || echo "No Data folder in publish"

# Runtime stage - Use lean ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /publish .

# Final verification of Data files
RUN echo "=== Final Data check ===" && \
    ls -la /app/Data/ || echo "No Data folder found"

# Port configuration - Koyeb expects port 8000
ENV ASPNETCORE_URLS=http://+:8000
ENV ASPNETCORE_ENVIRONMENT=Production

# Memory optimization for 1GB RAM instance
# GC Heap Hard Limit = 953MB (0x3B9ACA00) to prevent OOM on 1GB container
ENV DOTNET_GCHeapHardLimit=0x3B9ACA00

EXPOSE 8000

ENTRYPOINT ["dotnet", "LogosAPI.dll"]
```

---

## Memory Optimization for Koyeb

### GC Heap Hard Limit

When running on memory-constrained containers (like Koyeb's 1GB Small instance), set `DOTNET_GCHeapHardLimit` to prevent OOM kills:

| Instance Size | Recommended Limit | Hex Value |
|--------------|-------------------|-----------|
| 512MB (Free) | ~400MB | `0x19000000` |
| 1GB (Small) | ~953MB | `0x3B9ACA00` |
| 2GB (Medium) | ~1.8GB | `0x70000000` |

**Why:** .NET's GC doesn't know about container memory limits by default and may try to use more memory than available, causing the container to be OOM-killed.

**Calculation:** Leave 50-100MB headroom for stack, native allocations, and OS overhead.

---

## Critical: Include Data Files in Publish

Update `LogosAPI.csproj` to ensure Data files are copied.

**Important:** Use `Update` instead of `Include` to avoid duplicate content errors (SDK already includes Content items by default):

```xml
<ItemGroup>
  <Content Update="Data\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
  </Content>
</ItemGroup>
```

---

## Koyeb Configuration

### Service Settings
- **Deployment Method:** GitHub repository
- **Builder:** Dockerfile
- **Dockerfile Path:** `Dockerfile` (in root)
- **Port:** 8000 (critical - must match ASPNETCORE_URLS)
- **Health Check Path:** `/weatherforecast` or `/health`
- **Auto-deploy:** Enabled (deploys on git push to main)

### Instance Type
- **Free Tier:** 512MB RAM, shared CPU (sufficient for small APIs)
- **Nano:** $5/month, dedicated resources

### Environment Variables
- `ASPNETCORE_ENVIRONMENT=Production` (auto-set by Koyeb)
- Add custom vars as needed in Koyeb dashboard

---

## Persistent Storage (For User Data)

If the API needs to persist data:

### Adding a Volume in Koyeb
1. Go to Koyeb dashboard → Your service
2. Click "Settings" → "Edit Service"
3. Scroll to "Volumes" section
4. Click "Add Volume":
   - **Mount path:** `/app/Data` (or `/app/PersistentData`)
   - **Size:** 1 GB minimum
5. Save → Koyeb will redeploy

### Code Pattern for Persistent Data
```csharp
// Determine data path based on environment
string dataPath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production"
    ? "/app/Data"
    : Path.Combine(Directory.GetCurrentDirectory(), "Data");
```

---

## Program.cs Modifications for Koyeb

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Allow Swagger in all environments for Koyeb testing
app.UseSwagger();
app.UseSwaggerUI();

// Remove HTTPS redirect - Koyeb handles TLS at proxy level
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// Add health endpoint
app.MapGet("/health", () => "Healthy");

app.Run();
```

---

## Common Koyeb Issues & Solutions

### Issue: Data Files Not Found
**Symptom:** File not found exceptions at runtime

**Solution:**
1. Add `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` in .csproj
2. Add explicit COPY in Dockerfile
3. Add verification steps in Dockerfile to confirm files exist

### Issue: App Not Starting
**Symptom:** Health check fails, container restarts

**Solutions:**
1. Verify port is 8000 in both Dockerfile ENV and Koyeb settings
2. Check logs in Koyeb dashboard
3. Ensure entry point DLL name matches project name

### Issue: HTTPS Redirect Loop
**Symptom:** Infinite redirects, 502 errors

**Solution:** Remove `app.UseHttpsRedirection()` - Koyeb handles TLS

### Issue: Swagger Not Accessible
**Symptom:** Swagger UI returns 404 in production

**Solution:** Remove `IsDevelopment()` check around Swagger middleware

---

## Custom Domain Setup

### Adding Custom Domain to Koyeb
1. Purchase domain (Cloudflare, Porkbun, Namecheap)
2. In Koyeb dashboard → Service → Settings → Domains
3. Add custom domain (e.g., `api.yourdomain.net`)
4. Add DNS CNAME record at registrar:
   ```
   Type: CNAME
   Name: api
   Value: your-app-xxxx.koyeb.app
   ```
5. Wait for DNS propagation (5-30 minutes)

### Subdomain Strategy
One domain can serve multiple apps:
```
api.yourdomain.net    → LogosAPI
app.yourdomain.net    → Frontend
admin.yourdomain.net  → Admin panel
```

---

## Deployment Workflow

### Initial Deploy
1. Create `Dockerfile` in repo root
2. Push to GitHub
3. Create Koyeb service with GitHub connection
4. Configure port 8000
5. Wait for build (~2-5 minutes)

### Subsequent Deploys
1. Make changes locally
2. `git add -A && git commit -m "description"`
3. `git push origin main`
4. Koyeb auto-deploys (~1-3 minutes with cache)

### Monitoring
- Build logs: Koyeb dashboard → Deployments
- Runtime logs: Koyeb dashboard → Logs
- Metrics: Koyeb dashboard → Service overview

---

## Cost Summary

| Configuration | Monthly Cost |
|---------------|-------------|
| Free tier (no domain) | $0 |
| Free tier + custom domain | ~$1 ($10-15/year for domain) |
| Nano instance | $5/month |
| With volume storage | +$0.20/GB/month |

---

## Next Steps Checklist

- [ ] Create Dockerfile in repo root
- [ ] Update .csproj with Content includes for Data files
- [ ] Modify Program.cs (remove HTTPS redirect, add health endpoint)
- [ ] Push to GitHub
- [ ] Create Koyeb service
- [ ] Configure port 8000
- [ ] Test endpoints
- [ ] (Optional) Add persistent volume
- [ ] (Optional) Configure custom domain
