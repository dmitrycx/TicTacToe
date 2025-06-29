using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Tests.Features.SimulateGame;

[Trait("Category", "ContainerIntegration")]
public class SimulateGameContainerIntegrationTests
{
    private readonly HttpClient _gameSessionClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SimulateGameContainerIntegrationTests()
    {
        _gameSessionClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8081")
        };
    }

    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task SimulateGame_WithValidSession_ShouldCompleteSuccessfully()
    {
        // Arrange - Create a session first
        var createRequest = new { };
        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _gameSessionClient.PostAsync("/sessions", createContent);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(createResponseContent);
        var sessionId = jsonDoc.RootElement.GetProperty("sessionId").GetString();
        
        // Act - Simulate the game
        var simulateRequest = new { };
        var simulateJson = JsonSerializer.Serialize(simulateRequest);
        var simulateContent = new StringContent(simulateJson, Encoding.UTF8, "application/json");
        var simulateResponse = await _gameSessionClient.PostAsync($"/sessions/{sessionId}/simulate", simulateContent);
        
        // Assert
        simulateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var simulateResponseContent = await simulateResponse.Content.ReadAsStringAsync();
        var simulationDoc = JsonDocument.Parse(simulateResponseContent);
        
        // Check that winner and moves exist
        simulationDoc.RootElement.TryGetProperty("winner", out var winnerElement).Should().BeTrue();
        winnerElement.GetString().Should().NotBeNullOrEmpty();
        
        simulationDoc.RootElement.TryGetProperty("moves", out var movesElement).Should().BeTrue();
        movesElement.GetArrayLength().Should().BeGreaterThan(0);
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task SimulateGame_WithNonExistentSession_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _gameSessionClient.PostAsync($"/sessions/{nonExistentId}/simulate", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task SimulateGame_WithAlreadyCompletedSession_ShouldReturn400()
    {
        // Arrange - Create a session and simulate it once
        var createRequest = new { };
        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _gameSessionClient.PostAsync("/sessions", createContent);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(createResponseContent);
        var sessionId = jsonDoc.RootElement.GetProperty("sessionId").GetString();
        
        // Simulate the game once
        var simulateRequest = new { };
        var simulateJson = JsonSerializer.Serialize(simulateRequest);
        var simulateContent = new StringContent(simulateJson, Encoding.UTF8, "application/json");
        var simulateResponse = await _gameSessionClient.PostAsync($"/sessions/{sessionId}/simulate", simulateContent);
        simulateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Act - Try to simulate the same session again
        var secondSimulateResponse = await _gameSessionClient.PostAsync($"/sessions/{sessionId}/simulate", simulateContent);
        
        // Assert
        secondSimulateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
} 