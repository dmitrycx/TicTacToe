using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using FastEndpoints;
using FastEndpoints.Swagger;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// FastEndpoints registration
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

// Configure JSON serialization to use string enums
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configuration with Validation
builder.Services.AddOptions<GameEngineSettings>()
    .Bind(builder.Configuration.GetSection("GameEngine"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Infrastructure Services
builder.Services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();

// Move Generation Services
builder.Services.AddSingleton<IMoveGenerator, RandomMoveGenerator>();
builder.Services.AddSingleton<IMoveGenerator, RuleBasedMoveGenerator>();
builder.Services.AddSingleton<IMoveGeneratorFactory, MoveGeneratorFactory>();

// HTTP Client with Resilience (Polly)
builder.Services.AddHttpClient<IGameEngineApiClient, GameEngineHttpClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<GameEngineSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
})
.AddPolicyHandler(GetRetryPolicy());

var app = builder.Build();

// FastEndpoints
app.UseFastEndpoints();
app.UseSwaggerGen();

app.UseHttpsRedirection();

app.Run();

// Polly Retry Policy
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // Handles 5xx, 408 (Request Timeout)
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // Example: also retry on 404
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + 
            TimeSpan.FromMilliseconds(new Random().Next(0, 100))
        );
}

namespace TicTacToe.GameSession
{
    public partial class Program { }
}