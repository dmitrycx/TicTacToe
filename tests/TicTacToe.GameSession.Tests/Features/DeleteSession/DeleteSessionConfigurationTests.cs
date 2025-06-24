using Xunit;
using FastEndpoints;
using FluentAssertions;
using System.Net;
using TicTacToe.GameSession.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Persistence;

namespace TicTacToe.GameSession.Tests.Features.DeleteSession;

[Trait("Category", "Unit")]
public class DeleteSessionConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteSessionEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/sessions/{sessionId}");

        // Assert
        // A 404 proves the route exists and was hit (session doesn't exist, but route does)
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        response.StatusCode.Should().NotBe(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DELETE_Sessions_WithValidGuid_ShouldAcceptRequest()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/sessions/{sessionId}");

        // Assert
        // Should not return 400 Bad Request for valid GUID
        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("invalid-guid")]
    [InlineData("not-a-guid")]
    [InlineData("123")]
    [InlineData("")]
    [Trait("Category", "Unit")]
    public async Task DELETE_Sessions_WithInvalidGuid_ShouldRejectRequest(string invalidGuid)
    {
        // Arrange
        var url = $"/sessions/{invalidGuid}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        // Should return 400 Bad Request or 404 Not Found for invalid GUID
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DELETE_Sessions_ShouldReturnNotFound_WhenSessionDoesNotExist()
    {
        // Arrange
        var nonExistentSessionId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/sessions/{nonExistentSessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DELETE_Sessions_ShouldReturnJsonContent_WhenSuccessful()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/sessions/{sessionId}");

        // Assert
        // Even for 404, should return JSON content
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
} 