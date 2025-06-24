using Xunit;
using System.Text.Json;
using System.Text;
using TicTacToe.GameSession.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Persistence;

namespace TicTacToe.GameSession.Tests.Features.SimulateGame;

[Trait("Category", "Unit")]
public class SimulateGameConfigurationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SimulateGame_WithValidGuid_ShouldReturn404_WhenSessionDoesNotExist()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync($"/sessions/{sessionId}/simulate", content);
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode); // 404 because session doesn't exist, but route is configured
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public async Task SimulateGame_WithInvalidGuid_ShouldReturn404()
    {
        // Arrange
        var invalidGuid = "not-a-guid";
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync($"/sessions/{invalidGuid}/simulate", content);
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode); // FastEndpoints returns 404 for non-GUIDs
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SimulateGameEndpoint_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var app = fixture.CreateClient();
        var sessionId = Guid.NewGuid();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = app.PostAsync($"/sessions/{sessionId}/simulate", jsonContent).Result;

        // Assert
        response.Should().NotBeNull();
        // Should not return 404 for valid GUID format
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void POST_Sessions_Simulate_InvalidGuidFormat_ShouldReturn404()
    {
        // Arrange
        var app = fixture.CreateClient();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = app.PostAsync("/sessions/invalid-guid/simulate", jsonContent).Result;

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void POST_Sessions_Simulate_ShouldAcceptJsonContent()
    {
        // Arrange
        var app = fixture.CreateClient();
        var sessionId = Guid.NewGuid();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = app.PostAsync($"/sessions/{sessionId}/simulate", jsonContent).Result;

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.UnsupportedMediaType);
    }
} 