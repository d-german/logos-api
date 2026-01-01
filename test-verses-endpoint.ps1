# Test script for VersesController endpoints
# Assumes API is running on localhost:5133

Write-Host "Testing VersesController Endpoints" -ForegroundColor Cyan
Write-Host "===================================`n" -ForegroundColor Cyan

# Test POST endpoint
Write-Host "1. Testing POST /api/verses/lookup" -ForegroundColor Yellow
$postBody = @{
    verseReferences = @("Matt.1.1", "John.3.16", "Rom.3.23")
} | ConvertTo-Json

try {
    $postResponse = Invoke-RestMethod -Uri "http://localhost:5133/api/verses/lookup" `
        -Method Post `
        -Body $postBody `
        -ContentType "application/json"
    
    Write-Host "✓ POST Success:" -ForegroundColor Green
    $postResponse | ConvertTo-Json -Depth 10
} catch {
    Write-Host "✗ POST Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n---`n"

# Test GET endpoint
Write-Host "2. Testing GET /api/verses/lookup" -ForegroundColor Yellow
try {
    $getResponse = Invoke-RestMethod -Uri "http://localhost:5133/api/verses/lookup?verseReferences=Matt.1.1&verseReferences=John.3.16" `
        -Method Get
    
    Write-Host "✓ GET Success:" -ForegroundColor Green
    $getResponse | ConvertTo-Json -Depth 10
} catch {
    Write-Host "✗ GET Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n---`n"

# Test health endpoint
Write-Host "3. Testing GET /health" -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "http://localhost:5133/health" -Method Get
    Write-Host "✓ Health Check Success: $healthResponse" -ForegroundColor Green
} catch {
    Write-Host "✗ Health Check Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n===================================`n" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
