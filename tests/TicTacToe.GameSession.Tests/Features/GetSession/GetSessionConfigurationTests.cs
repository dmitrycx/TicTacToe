using Xunit;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

public class GetSessionConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task GetSession_WithValidGuid_ShouldReturn404_WhenSessionDoesNotExist()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/sessions/{sessionId}");
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode); // 404 because session doesn't exist, but route is configured
    }
    
    [Fact]
    public async Task GetSession_WithInvalidGuid_ShouldReturn404()
    {
        // Arrange
        var invalidGuid = "not-a-guid";
        
        // Act
        var response = await _client.GetAsync($"/sessions/{invalidGuid}");
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode); // FastEndpoints returns 404 for non-GUIDs
    }
} 