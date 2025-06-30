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
        
        // Small delay to ensure the session is fully created
        await Task.Delay(1000);
        
        // Act - List sessions with retry logic for CI timing issues
        HttpResponseMessage listResponse = null;
        var maxRetries = 3;
        var retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            listResponse = await _gameSessionClient.GetAsync("/sessions");
            
            if (listResponse.StatusCode == HttpStatusCode.OK)
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
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK, 
            $"List sessions failed after {maxRetries} attempts. Last status: {listResponse.StatusCode}");
        
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