using LogosAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register BibleDataService as Singleton - loads once, reused for all requests
builder.Services.AddSingleton<IBibleDataService, BibleDataService>();

// Register VerseReferenceNormalizer as Singleton - stateless, thread-safe
builder.Services.AddSingleton<IVerseReferenceNormalizer, VerseReferenceNormalizer>();

// Register StrongsNumberNormalizer as Singleton - stateless, thread-safe
builder.Services.AddSingleton<IStrongsNumberNormalizer, StrongsNumberNormalizer>();

// Register RmacParser as Singleton - stateless, thread-safe
builder.Services.AddSingleton<IRmacParser, RmacParser>();

// Register VerseLookupService as Singleton - orchestrates lookup operations
builder.Services.AddSingleton<IVerseLookupService, VerseLookupService>();

// Register CommentaryService with HttpClient for external HelloAO API calls
builder.Services.AddHttpClient<ICommentaryService, CommentaryService>();

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

// Privacy policy for ChatGPT GPT Store
app.MapGet("/privacy", () => Results.Content("""
<!DOCTYPE html>
<html>
<head><title>Privacy Policy - Logos Bible API</title></head>
<body>
<h1>Privacy Policy</h1>
<p><strong>Last updated:</strong> January 1, 2026</p>
<h2>Logos Bible API</h2>
<p>This API provides Bible verse lookup functionality with Greek/Hebrew lexicon data.</p>
<h3>Data Collection</h3>
<p>This API does not collect, store, or share any personal data. The API only processes Bible verse references submitted in requests and returns publicly available biblical text and lexicon data.</p>
<h3>Logging</h3>
<p>Standard server logs may temporarily record request information (IP addresses, timestamps) for operational purposes. These logs are not shared with third parties and are automatically deleted.</p>
<h3>Third-Party Services</h3>
<p>This API is hosted on Koyeb. Please refer to <a href="https://www.koyeb.com/docs/legal/privacy-policy">Koyeb's Privacy Policy</a> for their data handling practices.</p>
<h3>Contact</h3>
<p>For questions about this privacy policy, please contact the API maintainer.</p>
</body>
</html>
""", "text/html"));

app.Run();

// Make Program accessible for integration testing
public partial class Program { }