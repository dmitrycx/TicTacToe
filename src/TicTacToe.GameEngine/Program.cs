using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger; // Use the native swagger generator
using TicTacToe.GameEngine.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Aspire and Configuration
builder.AddServiceDefaults();
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("engine.appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"engine.appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 2. Core Frameworks: FastEndpoints & Swagger
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "gameengine-v1"; // Unique document name
        s.Title = "GameEngine API"; // Correct title
        s.Version = "v1";
        s.Description = "API for TicTacToe game engine operations.";
    };
});

// 3. Repository Configuration - In-Memory Only
builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();

// Use the consistent, working pattern for the UI
app.UseOpenApi();
app.UseSwaggerUi(c =>
{
    c.DocumentPath = "/swagger/gameengine-v1/swagger.json"; // Point to this service's doc
    c.DefaultModelsExpandDepth = -1;
    c.DocExpansion = "list";
});

app.UseFastEndpoints(c =>
{
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

app.MapDefaultEndpoints(); // For Aspire health checks

app.Run();

// Make Program accessible for testing
namespace TicTacToe.GameEngine {
    public partial class Program { }
}