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
        var nonExistentGameId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/games/{nonExistentGameId}");

        // Assert
        // Verify the endpoint is properly configured and returns JSON
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
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
        // Verify route constraint rejects invalid GUID format
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 