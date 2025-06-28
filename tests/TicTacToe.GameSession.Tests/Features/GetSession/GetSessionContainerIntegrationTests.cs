using System.Net;
using System.Text;
using System.Text.Json;

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
    public async Task GetSession_WithNonExistentSession_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var response = await _gameSessionClient.GetAsync($"/sessions/{nonExistentId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task GetSession_WithExistingSession_ShouldReturn200()
    {
        // Arrange - Create a session first
        var createRequest = new { };
        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _gameSessionClient.PostAsync("/sessions", createContent);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var sessionData = await createResponse.Content.ReadAsStringAsync();
        var session = JsonSerializer.Deserialize<dynamic>(sessionData, _jsonOptions);
        var sessionId = session.GetProperty("id").GetString();
        
        // Act
        var response = await _gameSessionClient.GetAsync($"/sessions/{sessionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var retrievedSession = JsonSerializer.Deserialize<dynamic>(responseContent, _jsonOptions);
        
        retrievedSession.Should().NotBeNull();
        retrievedSession.GetProperty("id").GetString().Should().Be(sessionId);
        retrievedSession.GetProperty("status").GetString().Should().Be("Created");
    }
} 