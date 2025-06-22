using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration with Validation
builder.Services.AddOptions<GameEngineSettings>()
    .Bind(builder.Configuration.GetSection("GameEngine"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Infrastructure Services
builder.Services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();

// HTTP Client with Resilience (Polly)
builder.Services.AddHttpClient<IGameEngineApiClient, GameEngineHttpClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<GameEngineSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
})
.AddPolicyHandler(GetRetryPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

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