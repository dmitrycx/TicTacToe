using Microsoft.Extensions.Options;
using FastEndpoints;
using FastEndpoints.Swagger;
using System.Text.Json.Serialization;
using Microsoft.Extensions.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Load session-specific configuration files
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("session.appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"session.appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// FastEndpoints registration
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

// Configure JSON serialization to use string enums
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Infrastructure Services
builder.Services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();

// Move Generation Services
builder.Services.AddSingleton<IMoveGenerator, RandomMoveGenerator>();
builder.Services.AddSingleton<IMoveGenerator, RuleBasedMoveGenerator>();
builder.Services.AddSingleton<IMoveGeneratorFactory, MoveGeneratorFactory>();

// HTTP Client with Aspire service discovery and built-in resilience
builder.Services.AddHttpClient<IGameEngineApiClient, GameEngineHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://gameengine");
});

var app = builder.Build();

// FastEndpoints
app.UseFastEndpoints();
app.UseSwaggerGen();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.Run();

namespace TicTacToe.GameSession
{
    public partial class Program { }
}