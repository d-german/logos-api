# Task Completion Checklist: Logos API

## When a Task is Completed

### 1. Build Verification
```powershell
# Ensure solution builds without errors
dotnet build logos-api.sln
```
**Expected**: Build succeeded. 0 Error(s)

### 2. Run Tests
```powershell
# Run all unit and integration tests
dotnet test
```
**Expected**: All tests pass with 0 failures

### 3. Code Quality Review
- [ ] Verify SOLID principles are maintained
- [ ] Check that static methods are used where appropriate
- [ ] Ensure nullable reference types are handled correctly
- [ ] Confirm XML documentation is present for public APIs
- [ ] Review for functional programming patterns (immutability, pure functions)

### 4. Service Registration (if adding new services)
- [ ] Add service registration in `Program.cs`
- [ ] Choose appropriate lifetime (Singleton recommended for stateless services)
- [ ] Ensure interface is implemented
- [ ] Verify dependency injection works in controllers

### 5. Data Changes (if modifying embedded resources)
- [ ] Ensure data files are in `LogosAPI/Data/` directory
- [ ] Verify `<EmbeddedResource>` entry in `.csproj`
- [ ] Rebuild to embed new resources
- [ ] Test data loading with DataIntegrityTests

### 6. API Changes (if adding/modifying endpoints)
- [ ] Test with local API running (`dotnet run --project LogosAPI/LogosAPI.csproj`)
- [ ] Verify Swagger UI documentation at `/swagger`
- [ ] Test with `test-verses-endpoint.ps1` or manual testing
- [ ] Consider integration tests in `LogosAPI.Tests`

### 7. Docker Verification (for deployment changes)
```powershell
# Build and test Docker image locally
docker build -t logos-api .
docker run -p 8000:8000 logos-api
# Test: curl http://localhost:8000/health
```

### 8. Git Workflow
```powershell
# Review changes
git status
git diff

# Stage and commit
git add .
git commit -m "Descriptive commit message"

# Push to remote
git push
```

## Code Review Self-Check
- [ ] No hardcoded values (use configuration or constants)
- [ ] Error handling is appropriate
- [ ] Logging is in place for important operations
- [ ] Thread safety considered for shared state
- [ ] Performance impact assessed (especially for singleton services)
- [ ] Memory usage is reasonable (important for 1GB container limit)

## Integration Testing Priority
Focus on testing:
- New controller endpoints
- Service methods with complex logic
- Data parsing and normalization
- Cross-service interactions

## Definition of Done
- ✅ Code builds successfully
- ✅ All tests pass
- ✅ Code follows project conventions
- ✅ XML documentation added for public APIs
- ✅ Changes committed with clear message
- ✅ Integration tests added/updated if needed
