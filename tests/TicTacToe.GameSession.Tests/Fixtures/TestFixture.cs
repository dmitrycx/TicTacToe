using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TicTacToe.GameEngine.Persistence;

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
            services.AddSingleton(mockGameEngineClient.Object);
            
            // Register mock GameEngine dependencies to prevent DI resolution errors
            // These are needed because GameSession references GameEngine domain types
            var mockGameRepository = new Mock<IGameRepository>();
            services.AddSingleton(mockGameRepository.Object);
        });
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add test configuration
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"GameEngine:BaseUrl", "http://localhost:5000"},
                {"GameEngine:TimeoutSeconds", "30"}
            });
        });
    }
} 