var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

// Health endpoint for Koyeb health checks
app.MapGet("/health", () => "Healthy");

app.Run();