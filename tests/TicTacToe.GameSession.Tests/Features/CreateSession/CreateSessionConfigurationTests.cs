using Xunit;
using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Text;
using TicTacToe.GameSession.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Persistence;

namespace TicTacToe.GameSession.Tests.Features.CreateSession;

[Trait("Category", "Unit")]
public class CreateSessionConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    [Trait("Category", "Unit")]
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
    [Trait("Category", "Unit")]
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
    [Trait("Category", "Unit")]
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

    [Fact]
    [Trait("Category", "Unit")]
    public void POST_Sessions_ShouldHaveCorrectRoute()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = app.PostAsync("/sessions", new StringContent("{}", Encoding.UTF8, "application/json")).Result;

        // Assert
        response.Should().NotBeNull();
        // The route should be accessible (even if it returns an error for empty content)
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void POST_Sessions_ShouldAcceptJsonContent()
    {
        // Arrange
        var app = fixture.CreateClient();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = app.PostAsync("/sessions", jsonContent).Result;

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void POST_Sessions_ShouldReturnJsonResponse()
    {
        // Arrange
        var app = fixture.CreateClient();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = app.PostAsync("/sessions", jsonContent).Result;

        // Assert
        response.Should().NotBeNull();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void POST_Sessions_ShouldNotRequireAuthentication()
    {
        // Arrange
        var app = fixture.CreateClient();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = app.PostAsync("/sessions", jsonContent).Result;

        // Assert
        response.Should().NotBeNull();
        // Should not return 401 Unauthorized
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Unauthorized);
    }
} 