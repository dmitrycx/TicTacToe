using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

[Trait("Category", "ContainerIntegration")]
public class GetSessionContainerIntegrationTests
{
    private readonly HttpClient _gameSessionClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public GetSessionContainerIntegrationTests()
    {
        _gameSessionClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8081")
        };
    }

    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task GetSession_WithExistingSession_ShouldReturn200()
    {
        // Arrange - First create a session
        var createRequest = new { };
        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _gameSessionClient.PostAsync("/sessions", createContent);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(createResponseContent);
        var sessionId = jsonDoc.RootElement.GetProperty("sessionId").GetString();
        
        // Act - Get the created session
        var getResponse = await _gameSessionClient.GetAsync($"/sessions/{sessionId}");
        
        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var getResponseContent = await getResponse.Content.ReadAsStringAsync();
        var sessionDoc = JsonDocument.Parse(getResponseContent);
        var sessionIdElement = sessionDoc.RootElement.GetProperty("sessionId");
        
        sessionIdElement.GetString().Should().Be(sessionId);
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task GetSession_WithNonExistentSession_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var response = await _gameSessionClient.GetAsync($"/sessions/{nonExistentId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 