using Xunit;
using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Text;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.CreateSession;

public class CreateSessionConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task CreateSessionEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var request = new { };

        // Act
        var response = await _client.PostAsync("/sessions",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        // A 201 proves the route exists and was hit
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);

        // Should return JSON content
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task POST_Sessions_ShouldAcceptJsonContent()
    {
        // Arrange
        var request = new { };

        // Act
        var response = await _client.PostAsync("/sessions",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task POST_Sessions_ShouldReturnCreatedStatus()
    {
        // Arrange
        var request = new { };

        // Act
        var response = await _client.PostAsync("/sessions",
            new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        // Should return 201 Created for successful session creation
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
} 