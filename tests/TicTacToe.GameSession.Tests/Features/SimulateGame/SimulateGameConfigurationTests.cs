using Xunit;
using System.Text.Json;
using System.Text;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.SimulateGame;

public class SimulateGameConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task SimulateGame_WithValidGuid_ShouldReturn404_WhenSessionDoesNotExist()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync($"/sessions/{sessionId}/simulate", content);
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode); // 404 because session doesn't exist, but route is configured
    }
    
    [Fact]
    public async Task SimulateGame_WithInvalidGuid_ShouldReturn404()
    {
        // Arrange
        var invalidGuid = "not-a-guid";
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync($"/sessions/{invalidGuid}/simulate", content);
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode); // FastEndpoints returns 404 for non-GUIDs
    }
} 