using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToe.GameSession.Endpoints;

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
        var jsonDoc = JsonDocument.Parse(responseContent);
        var sessionsArray = jsonDoc.RootElement.GetProperty("sessions");
        
        sessionsArray.GetArrayLength().Should().Be(0);
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task ListSessions_WithCreatedSessions_ShouldReturnSessions()
    {
        // Arrange - Create a session first
        var createRequest = new { };
        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _gameSessionClient.PostAsync("/sessions", createContent);
        
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Act - List sessions
        var listResponse = await _gameSessionClient.GetAsync("/sessions");
        
        // Assert
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var listResponseContent = await listResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(listResponseContent);
        var sessionsArray = jsonDoc.RootElement.GetProperty("sessions");
        
        sessionsArray.GetArrayLength().Should().BeGreaterThan(0);
        
        // Check that at least one session has the expected status
        var hasCreatedSession = false;
        foreach (var session in sessionsArray.EnumerateArray())
        {
            if (session.TryGetProperty("status", out var statusElement) && 
                statusElement.GetString() == "Created")
            {
                hasCreatedSession = true;
                break;
            }
        }
        hasCreatedSession.Should().BeTrue();
    }
} 