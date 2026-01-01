using LogosAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register BibleDataService as Singleton - loads once, reused for all requests
builder.Services.AddSingleton<IBibleDataService, BibleDataService>();

// Register VerseReferenceNormalizer as Singleton - stateless, thread-safe
builder.Services.AddSingleton<IVerseReferenceNormalizer, VerseReferenceNormalizer>();

// Register VerseLookupService as Singleton - orchestrates lookup operations
builder.Services.AddSingleton<IVerseLookupService, VerseLookupService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in all environments for Koyeb testing
app.UseSwagger();
app.UseSwaggerUI();

// Note: HTTPS redirect removed - Koyeb handles TLS at proxy level
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Health endpoints for Koyeb health checks (both paths for flexibility)
app.MapGet("/health", () => "Healthy");
app.MapGet("/_health", () => "Healthy");

app.Run();

// Make Program accessible for integration testing
public partial class Program { }