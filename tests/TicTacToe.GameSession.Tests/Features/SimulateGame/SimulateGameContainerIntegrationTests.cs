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
        
        // Small delay to ensure the session is fully created
        await Task.Delay(1000);
        
        // Act - Simulate the game with retry logic for CI timing issues
        var simulateRequest = new { };
        var simulateJson = JsonSerializer.Serialize(simulateRequest);
        var simulateContent = new StringContent(simulateJson, Encoding.UTF8, "application/json");
        
        HttpResponseMessage simulateResponse = null;
        var maxRetries = 3;
        var retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            simulateResponse = await _gameSessionClient.PostAsync($"/sessions/{sessionId}/simulate", simulateContent);
            
            if (simulateResponse.StatusCode == HttpStatusCode.OK)
            {
                break;
            }
            
            retryCount++;
            if (retryCount < maxRetries)
            {
                await Task.Delay(2000); // Wait 2 seconds before retry
            }
        }
        
        // Assert
        simulateResponse.StatusCode.Should().Be(HttpStatusCode.OK, 
            $"Simulation failed after {maxRetries} attempts. Last status: {simulateResponse.StatusCode}");
        
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
    public async Task SimulateGame_WithAlreadyCompletedSession_ShouldReturn200()
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
        
        // Assert - Re-simulation should be allowed and return 200 OK
        secondSimulateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify the second simulation returns the existing game state
        var secondSimulateResponseContent = await secondSimulateResponse.Content.ReadAsStringAsync();
        var secondSimulationDoc = JsonDocument.Parse(secondSimulateResponseContent);
        
        // Check that the response contains the expected fields
        secondSimulationDoc.RootElement.TryGetProperty("sessionId", out var sessionIdElement).Should().BeTrue();
        sessionIdElement.GetString().Should().Be(sessionId);
        
        secondSimulationDoc.RootElement.TryGetProperty("isCompleted", out var isCompletedElement).Should().BeTrue();
        isCompletedElement.GetBoolean().Should().BeTrue();
        
        secondSimulationDoc.RootElement.TryGetProperty("moves", out var movesElement).Should().BeTrue();
        movesElement.GetArrayLength().Should().BeGreaterThan(0);
        
        // Winner can be null (for a draw) or a player name, both are valid
        secondSimulationDoc.RootElement.TryGetProperty("winner", out var winnerElement).Should().BeTrue();
        // Don't assert on winner value since it can be null for a draw
    }
} 