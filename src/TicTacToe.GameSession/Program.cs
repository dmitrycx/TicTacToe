using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger; // Use the native swagger generator
using TicTacToe.GameSession.Hubs;
using TicTacToe.GameSession.Services;
using TicTacToe.GameSession.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Aspire and Configuration
builder.AddServiceDefaults();
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("session.appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"session.appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 2. Core Frameworks: FastEndpoints & Swagger
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "gamesession-v1"; // Use the options to set the name
        s.Title = "GameSession API";
        s.Version = "v1";
    };
});

// 3. Application-Specific Services
builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

var allowedOrigins = builder.Configuration["AllowedCorsOrigins"]?.Split(';') ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    
    // Production-ready development policy
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development: Allow localhost with any port
            policy.SetIsOriginAllowed(origin => 
                origin.StartsWith("http://localhost:") || 
                origin.StartsWith("https://localhost:") ||
                origin.StartsWith("http://127.0.0.1:") ||
                origin.StartsWith("https://127.0.0.1:")
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        }
        else
        {
            // In production: Use strict origin validation
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

builder.Services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();
builder.Services.AddSingleton<IMoveGenerator, RandomMoveGenerator>();
builder.Services.AddSingleton<IMoveGenerator, RuleBasedMoveGenerator>();
builder.Services.AddSingleton<IMoveGenerator, AIMoveGenerator>();
builder.Services.AddSingleton<IMoveGeneratorFactory, MoveGeneratorFactory>();
builder.Services.AddScoped<ISignalRNotificationService, SignalRNotificationService>();

// Configure HttpClient for GameEngine with proper SSL certificate handling for development
builder.Services.AddHttpClient<IGameEngineApiClient, GameEngineHttpClient>(client =>
{
    var gameEngineUrl = builder.Configuration["GameEngineServiceUrl"] ?? "http://gameengine";
    client.BaseAddress = new Uri(gameEngineUrl);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Disable HTTPS redirection only in development to avoid certificate issues
    // In production, this should be enabled with proper SSL certificates
}
else
{
    app.UseHttpsRedirection(); // Enable HTTPS redirection in production
}
app.UseRouting();

// Add CORS middleware here - it must be between UseRouting and UseEndpoints/UseFastEndpoints
app.UseCors("DevelopmentPolicy"); // Use the more permissive policy

// This is the correct combination for the "pretty" NSwag UI
// powered by the reliable FastEndpoints generator.
app.UseOpenApi();
app.UseSwaggerUi(c =>
{
    c.DocumentPath = "/swagger/gamesession-v1/swagger.json";
    c.DefaultModelsExpandDepth = -1;
    c.DocExpansion = "list";
});

app.UseFastEndpoints(c =>
{
    // This setting should ideally be done once at the service level if needed
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

app.MapHub<GameHub>("/gameHub").RequireCors("DevelopmentPolicy");
app.MapDefaultEndpoints(); // For Aspire health checks

app.Run();

// Make Program accessible for testing
namespace TicTacToe.GameSession {
    public partial class Program { }
}