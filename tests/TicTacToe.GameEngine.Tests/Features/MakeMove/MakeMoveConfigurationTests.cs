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
        var nonExistentGameId = Guid.NewGuid();
        var moveRequest = new { Row = 0, Column = 0 };

        // Act
        var response = await _client.PostAsync($"/games/{nonExistentGameId}/move",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(moveRequest), Encoding.UTF8, "application/json"));

        // Assert
        // Verify the endpoint is properly configured and returns JSON
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
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
        // Verify route constraint rejects invalid GUID format
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