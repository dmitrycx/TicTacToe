using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger; // Use the native swagger generator
using TicTacToe.GameSession.Hubs;
using TicTacToe.GameSession.Services;

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
});

builder.Services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();
builder.Services.AddSingleton<IMoveGenerator, RandomMoveGenerator>();
builder.Services.AddSingleton<IMoveGenerator, RuleBasedMoveGenerator>();
builder.Services.AddSingleton<IMoveGeneratorFactory, MoveGeneratorFactory>();
builder.Services.AddScoped<ISignalRNotificationService, SignalRNotificationService>();
builder.Services.AddHttpClient<IGameEngineApiClient, GameEngineHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://gameengine");
});


var app = builder.Build();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseRouting();

// Add CORS middleware here - it must be between UseRouting and UseEndpoints/UseFastEndpoints
app.UseCors("SignalRPolicy");

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

app.MapHub<GameHub>("/gameHub").RequireCors("SignalRPolicy");
app.MapDefaultEndpoints(); // For Aspire health checks

app.Run();

// Make Program accessible for testing
public partial class Program { }