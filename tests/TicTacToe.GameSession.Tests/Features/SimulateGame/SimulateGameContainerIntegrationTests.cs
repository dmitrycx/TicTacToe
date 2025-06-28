using System.Net;
using System.Text;
using System.Text.Json;

namespace TicTacToe.GameSession.Tests.Features.SimulateGame;

[Trait("Category", "ContainerIntegration")]
public class SimulateGameContainerIntegrationTests
{
    private readonly HttpClient _gameSessionClient;
    private readonly HttpClient _gameEngineClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SimulateGameContainerIntegrationTests()
    {
        _gameSessionClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8081")
        };
        
        _gameEngineClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8080")
        };
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
    public async Task SimulateGame_WithValidSession_ShouldCompleteSuccessfully()
    {
        // Arrange - Create a new session
        var createSessionRequest = new { };
        var createJson = JsonSerializer.Serialize(createSessionRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _gameSessionClient.PostAsync("/sessions", createContent);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var sessionData = await createResponse.Content.ReadAsStringAsync();
        var session = JsonSerializer.Deserialize<dynamic>(sessionData, _jsonOptions);
        var sessionId = session.GetProperty("id").GetString();
        
        // Act - Simulate the game
        var simulateRequest = new { };
        var simulateJson = JsonSerializer.Serialize(simulateRequest);
        var simulateContent = new StringContent(simulateJson, Encoding.UTF8, "application/json");
        var simulateResponse = await _gameSessionClient.PostAsync($"/sessions/{sessionId}/simulate", simulateContent);
        
        // Assert
        simulateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify the session is now completed
        var getResponse = await _gameSessionClient.GetAsync($"/sessions/{sessionId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var sessionDetails = await getResponse.Content.ReadAsStringAsync();
        var sessionDetailsObj = JsonSerializer.Deserialize<dynamic>(sessionDetails, _jsonOptions);
        sessionDetailsObj.GetProperty("status").GetString().Should().Be("Completed");
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task SimulateGame_WithAlreadyCompletedSession_ShouldReturn400()
    {
        // Arrange - Create and complete a session
        var createSessionRequest = new { };
        var createJson = JsonSerializer.Serialize(createSessionRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _gameSessionClient.PostAsync("/sessions", createContent);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var sessionData = await createResponse.Content.ReadAsStringAsync();
        var session = JsonSerializer.Deserialize<dynamic>(sessionData, _jsonOptions);
        var sessionId = session.GetProperty("id").GetString();
        
        // Complete the session first
        var simulateRequest = new { };
        var simulateJson = JsonSerializer.Serialize(simulateRequest);
        var simulateContent = new StringContent(simulateJson, Encoding.UTF8, "application/json");
        var simulateResponse = await _gameSessionClient.PostAsync($"/sessions/{sessionId}/simulate", simulateContent);
        simulateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Act - Try to simulate again
        var secondSimulateResponse = await _gameSessionClient.PostAsync($"/sessions/{sessionId}/simulate", simulateContent);
        
        // Assert
        secondSimulateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
} 