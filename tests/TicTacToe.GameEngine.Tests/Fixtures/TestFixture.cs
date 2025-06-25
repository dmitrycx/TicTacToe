using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using TicTacToe.GameEngine.Persistence;

namespace TicTacToe.GameEngine.Tests.Fixtures;

public class TestFixture : WebApplicationFactory<Program>
{
    public IGameRepository GameRepository => Services.GetRequiredService<IGameRepository>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // For integration tests, use the real in-memory repository
            // This ensures state persists across HTTP requests within the same test
            services.AddSingleton<IGameRepository, InMemoryGameRepository>();
        });
    }
} 