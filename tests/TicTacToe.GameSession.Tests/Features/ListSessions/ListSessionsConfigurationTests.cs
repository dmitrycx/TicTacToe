using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.ListSessions;

[Trait("Category", "Unit")]
public class ListSessionsConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task ListSessionsEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = await app.GetAsync("/sessions");

        // Assert
        response.Should().NotBeNull();
        // Should return 200 OK for valid request
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GET_Sessions_ShouldReturnJsonResponse()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = await app.GetAsync("/sessions");

        // Assert
        response.Should().NotBeNull();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GET_Sessions_ShouldNotRequireAuthentication()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = await app.GetAsync("/sessions");

        // Assert
        response.Should().NotBeNull();
        // Should not return 401 Unauthorized
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
} 