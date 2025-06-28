using System.Net;
using System.Text;
using System.Text.Json;

namespace TicTacToe.GameSession.Tests.Features.ListSessions;

[Trait("Category", "ContainerIntegration")]
public class ListSessionsContainerIntegrationTests
{
    private readonly HttpClient _gameSessionClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ListSessionsContainerIntegrationTests()
    {
        _gameSessionClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8081")
        };
    }

    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task ListSessions_WithNoSessions_ShouldReturnEmptyList()
    {
        // Act
        var response = await _gameSessionClient.GetAsync("/sessions");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var sessions = JsonSerializer.Deserialize<dynamic>(responseContent, _jsonOptions);
        
        sessions.Should().NotBeNull();
        sessions.GetArrayLength().Should().Be(0);
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task ListSessions_WithCreatedSessions_ShouldReturnSessions()
    {
        // Arrange - Create some sessions
        var createRequest = new { };
        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        
        await _gameSessionClient.PostAsync("/sessions", createContent);
        await _gameSessionClient.PostAsync("/sessions", createContent);
        
        // Act
        var response = await _gameSessionClient.GetAsync("/sessions");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var sessions = JsonSerializer.Deserialize<dynamic>(responseContent, _jsonOptions);
        
        sessions.Should().NotBeNull();
        sessions.GetArrayLength().Should().BeGreaterThanOrEqualTo(2);
        
        // Verify each session has required properties
        for (int i = 0; i < sessions.GetArrayLength(); i++)
        {
            var session = sessions[i];
            session.GetProperty("id").GetString().Should().NotBeNullOrEmpty();
            session.GetProperty("status").GetString().Should().NotBeNullOrEmpty();
        }
    }
} 