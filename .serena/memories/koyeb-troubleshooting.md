# Koyeb Troubleshooting Guide

**Last Updated:** January 1, 2026  
**Applies To:** ASP.NET Core applications deployed to Koyeb

---

## Common Deployment Issues

### 1. Build Fails - Files Not Found

**Symptom:** Build fails with "file not found" errors for data files

**Root Cause:** Data files not being copied to publish output

**Solution:**
```xml
<!-- Add to .csproj -->
<!-- Use Update (not Include) to avoid NETSDK1022 duplicate items error -->
<ItemGroup>
  <Content Update="Data\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
  </Content>
</ItemGroup>
```

**Dockerfile verification:**
```dockerfile
# Add verification steps to see what's in publish
RUN echo "=== Checking publish output ===" && \
    ls -la /publish/ && \
    ls -la /publish/Data/ || echo "No Data folder"
```

---

### 2. Container Starts But Health Check Fails

**Symptom:** Container starts, then restarts repeatedly

**Root Causes:**
1. Port mismatch
2. No health endpoint
3. Application crash on startup

**Solutions:**

**Port Configuration (must be 8000):**
```dockerfile
ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000
```

**Add Health Endpoint:**
```csharp
// In Program.cs
app.MapGet("/health", () => "Healthy");
```

**In Koyeb Dashboard:**
- Health Check Path: `/health`
- Health Check Port: `8000`

---

### 3. HTTPS Redirect Loop / 502 Errors

**Symptom:** Infinite redirects or 502 Bad Gateway

**Root Cause:** `UseHttpsRedirection()` conflicts with Koyeb's TLS termination

**Solution:**
```csharp
// Remove or comment out this line in Program.cs
// app.UseHttpsRedirection();
```

Koyeb handles HTTPS at the proxy level - your app should serve HTTP only.

---

### 4. Swagger Not Working in Production

**Symptom:** Swagger UI returns 404 when deployed

**Root Cause:** Swagger middleware wrapped in `IsDevelopment()` check

**Solution:**
```csharp
// Change from:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Change to (enable for all environments):
app.UseSwagger();
app.UseSwaggerUI();
```

---

### 5. Data Files Not Persisting

**Symptom:** User data lost after redeployment

**Root Cause:** Container filesystem is ephemeral

**Solution:** Add Koyeb Persistent Volume
1. Koyeb Dashboard → Service → Settings
2. Add Volume: Mount path `/app/PersistentData`, Size 1GB
3. Update code to use persistent path for mutable data

---

### 6. Application Name/URL Issues

**Symptom:** Auto-generated Koyeb URL is undesirable

**Root Cause:** Koyeb generates random URL suffix

**Solutions:**
1. **Delete and recreate service** with preferred name
2. **Add custom domain** to override auto-generated URL

---

### 7. Build Cache Issues

**Symptom:** Changes not reflected after deployment

**Root Cause:** Docker/Koyeb caching previous build

**Solutions:**

**Force cache invalidation in Dockerfile:**
```dockerfile
# Add before COPY steps
ARG CACHEBUST=1
```

**Or add timestamp:**
```dockerfile
RUN echo "Build timestamp: $(date)"
COPY . .
```

---

### 8. DLL Name Mismatch

**Symptom:** `Could not find the specified assembly`

**Root Cause:** ENTRYPOINT DLL name doesn't match project name

**Solution:**
```dockerfile
# DLL name must match ProjectName.dll
ENTRYPOINT ["dotnet", "LogosAPI.dll"]
```

Check your `.csproj` file for the actual assembly name.

---

### 9. Memory/Resource Limits

**Symptom:** Container killed, "OOMKilled" in logs

**Root Cause:** Application exceeds free tier memory limits

**Solutions:**
1. Optimize memory usage
2. Reduce concurrent connections
3. Upgrade to paid tier (Nano: 512MB dedicated)

---

### 10. Environment Variables Not Working

**Symptom:** Configuration values not being read

**Root Cause:** Environment variable names don't match expected format

**Solution:**
ASP.NET Core uses `__` (double underscore) for nested config:
```
# For appsettings section "Database:ConnectionString"
Environment variable: Database__ConnectionString
```

---

## Diagnostic Commands

### Local Testing
```bash
# Build Docker image locally
docker build -t logosapi .

# Run locally
docker run -p 8000:8000 logosapi

# Test endpoints
curl http://localhost:8000/health
curl http://localhost:8000/weatherforecast
```

### Check Koyeb Logs
1. Dashboard → Service → Logs tab
2. Filter by: Build logs, Runtime logs, Error logs

### Verify Files in Container
Add to Dockerfile for debugging:
```dockerfile
RUN find /app -name "*.json" -type f
RUN ls -laR /app/
```

---

## Quick Reference

| Issue | Quick Fix |
|-------|-----------|
| Files not found | Add `<CopyToPublishDirectory>` to .csproj |
| Health check fails | Add `/health` endpoint, verify port 8000 |
| HTTPS redirect loop | Remove `UseHttpsRedirection()` |
| Swagger 404 | Enable Swagger for all environments |
| Data not persisting | Add Koyeb persistent volume |
| Wrong URL | Delete/recreate service or add custom domain |
| Build cache | Add `ARG CACHEBUST` to Dockerfile |

---

## Getting Help

1. **Koyeb Documentation:** https://www.koyeb.com/docs
2. **Koyeb Discord:** Community support
3. **ASP.NET Core Docs:** https://docs.microsoft.com/aspnet/core
4. **Docker Docs:** https://docs.docker.com

---

## Related Memories

- `koyeb-deployment-guide.md` - Step-by-step deployment
- `project-overview.md` - Project structure
