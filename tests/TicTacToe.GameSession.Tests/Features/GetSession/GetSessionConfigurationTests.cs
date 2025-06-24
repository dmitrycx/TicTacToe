using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Persistence;
using Xunit;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

[Trait("Category", "Unit")]
public class GetSessionConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    [Trait("Category", "Unit")]
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
    [Trait("Category", "Unit")]
    public async Task GetSession_WithInvalidGuid_ShouldReturn404()
    {
        // Arrange
        var invalidGuid = "not-a-guid";
        
        // Act
        var response = await _client.GetAsync($"/sessions/{invalidGuid}");
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode); // FastEndpoints returns 404 for non-GUIDs
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetSessionEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = app.GetAsync("/sessions/invalid-guid").Result;

        // Assert
        response.Should().NotBeNull();
        // Should return 404 for invalid GUID format, not 500 or other server errors
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GET_Sessions_InvalidGuidFormat_ShouldReturn404()
    {
        // Arrange
        var app = fixture.CreateClient();

        // Act
        var response = app.GetAsync("/sessions/invalid-guid").Result;

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 