using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Tests.Fixtures;

public class TestFixture : WebApplicationFactory<Program>
{
    public IGameSessionRepository GameSessionRepository => Services.GetRequiredService<IGameSessionRepository>();
    public IGameEngineApiClient GameEngineApiClient => Services.GetRequiredService<IGameEngineApiClient>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // For integration tests, use the real in-memory repository
            // This ensures state persists across HTTP requests within the same test
            services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();
            
            // Use a mock GameEngine API client for integration tests
            // This allows us to control external service responses
            var mockGameEngineClient = new Mock<IGameEngineApiClient>();
            
            // Configure mock responses for simulation tests
            mockGameEngineClient
                .Setup(x => x.CreateGameAsync())
                .ReturnsAsync(new CreateGameResponse(Guid.NewGuid(), DateTime.UtcNow));
            
            mockGameEngineClient
                .Setup(x => x.GetGameStateAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GameStateResponse(
                    Guid.NewGuid(),
                    "InProgress",
                    "X",
                    null,
                    new List<List<string?>> 
                    { 
                        new() { null, null, null },
                        new() { null, null, null },
                        new() { null, null, null }
                    },
                    DateTime.UtcNow,
                    null));
            
            mockGameEngineClient
                .Setup(x => x.MakeMoveAsync(It.IsAny<Guid>(), It.IsAny<MakeMoveRequest>()))
                .ReturnsAsync(new GameStateResponse(
                    Guid.NewGuid(),
                    "Win",
                    "X",
                    "X",
                    new List<List<string?>> 
                    { 
                        new() { "X", "O", "X" },
                        new() { "O", "X", "O" },
                        new() { "X", null, null }
                    },
                    DateTime.UtcNow,
                    DateTime.UtcNow));
            
            services.AddSingleton(mockGameEngineClient.Object);
            
            // Register mock GameEngine dependencies to prevent DI resolution errors
            // These are needed because GameSession references GameEngine domain types
            var mockGameRepository = new Mock<IGameRepository>();
            services.AddSingleton(mockGameRepository.Object);
            
            // Configure JSON serialization to match the backend
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        });
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add test configuration
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"GameEngine:BaseUrl", "http://localhost:5000"},
                {"GameEngine:TimeoutSeconds", "30"}
            });
        });
    }
} 