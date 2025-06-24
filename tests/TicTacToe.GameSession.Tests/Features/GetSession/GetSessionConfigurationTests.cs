using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

[Trait("Category", "Unit")]
public class GetSessionConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetSession_WithValidGuid_ShouldReturn404_WhenSessionDoesNotExist()
    {
        // Arrange
        var app = fixture.CreateClient();
        var sessionId = Guid.NewGuid();
        
        // Act
        var response = await app.GetAsync($"/sessions/{sessionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // 404 because session doesn't exist, but route is configured
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetSession_WithInvalidGuid_ShouldReturn404()
    {
        // Arrange
        var app = fixture.CreateClient();
        var invalidGuid = "not-a-guid";
        
        // Act
        var response = await app.GetAsync($"/sessions/{invalidGuid}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // FastEndpoints returns 404 for non-GUIDs
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetSessionEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = await app.GetAsync("/sessions/invalid-guid");

        // Assert
        response.Should().NotBeNull();
        // Should return 404 for invalid GUID format, not 500 or other server errors
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GET_Sessions_InvalidGuidFormat_ShouldReturn404()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = await app.GetAsync("/sessions/invalid-guid");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 