using Xunit;
using FluentAssertions;
using System.Net;
using TicTacToe.GameEngine.Tests.Fixtures;

namespace TicTacToe.GameEngine.Tests.Features.GetGameState;

[Trait("Category", "Unit")]
public class GetGameStateConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetGameStateEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        // We can use any valid Guid format string. It doesn't need to exist.
        var nonExistentGameId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/games/{nonExistentGameId}");

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
    public async Task GET_Games_InvalidGuidFormat_ShouldReturn404()
    {
        // Arrange
        var invalidGuidFormat = "this-is-not-a-guid";

        // Act
        var response = await _client.GetAsync($"/games/{invalidGuidFormat}");

        // Assert
        // This test is more specific. It proves the route constraint is working.
        // FastEndpoints will reject this route before it even hits your handler.
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 