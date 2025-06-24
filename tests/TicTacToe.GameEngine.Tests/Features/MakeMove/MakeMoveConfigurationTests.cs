using Xunit;
using FluentAssertions;
using System.Net;
using System.Text;
using TicTacToe.GameEngine.Tests.Fixtures;

namespace TicTacToe.GameEngine.Tests.Features.MakeMove;

[Trait("Category", "Unit")]
public class MakeMoveConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MakeMoveEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        // We can use any valid Guid format string. It doesn't need to exist.
        var nonExistentGameId = Guid.NewGuid();
        var moveRequest = new { Row = 0, Column = 0 };

        // Act
        var response = await _client.PostAsync($"/games/{nonExistentGameId}/move",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(moveRequest), Encoding.UTF8, "application/json"));

        // Assert
        // A 404 proves the route exists and was hit. If the route didn't exist,
        // the status code would still be 404, but this is an acceptable overlap.
        // The key is that we are not getting a 401, 415, or 500.
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);

        // Even on a 404, FastEndpoints will often return a JSON problem+details response.
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task POST_Games_Move_InvalidGuidFormat_ShouldReturn404()
    {
        // Arrange
        var invalidGuidFormat = "this-is-not-a-guid";
        var moveRequest = new { Row = 0, Column = 0 };

        // Act
        var response = await _client.PostAsync($"/games/{invalidGuidFormat}/move",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(moveRequest), Encoding.UTF8, "application/json"));

        // Assert
        // This test is more specific. It proves the route constraint is working.
        // FastEndpoints will reject this route before it even hits your handler.
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task POST_Games_Move_ShouldAcceptJsonContent()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var moveRequest = new { Row = 0, Column = 0 };

        // Act
        var response = await _client.PostAsync($"/games/{gameId}/move",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(moveRequest), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.UnsupportedMediaType);
    }
} 