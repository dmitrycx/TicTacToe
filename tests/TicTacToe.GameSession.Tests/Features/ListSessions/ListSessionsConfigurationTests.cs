using Xunit;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.ListSessions;

public class ListSessionsConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task ListSessions_ShouldReturn200_WhenRouteIsConfigured()
    {
        // Act
        var response = await _client.GetAsync("/sessions");
        
        // Assert
        Assert.Equal(200, (int)response.StatusCode);
    }
} 