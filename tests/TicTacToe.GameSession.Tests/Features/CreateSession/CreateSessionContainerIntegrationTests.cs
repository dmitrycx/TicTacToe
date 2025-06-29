namespace TicTacToe.GameSession.Tests.Features.CreateSession;

[Trait("Category", "ContainerIntegration")]
public class CreateSessionContainerIntegrationTests
{
    private readonly HttpClient _gameSessionClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CreateSessionContainerIntegrationTests()
    {
        _gameSessionClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8081")
        };
    }

    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task CreateSession_WithValidRequest_ShouldReturn201()
    {
        // Arrange
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await _gameSessionClient.PostAsync("/sessions", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        // Debug: Let's see what the actual JSON looks like
        Console.WriteLine($"Response JSON: {responseContent}");
        
        // Try to deserialize as JsonDocument first to see the structure
        var jsonDoc = JsonDocument.Parse(responseContent);
        var root = jsonDoc.RootElement;
        
        // Check if properties exist
        if (root.TryGetProperty("sessionId", out var sessionIdElement))
        {
            sessionIdElement.GetString().Should().NotBeNullOrEmpty();
        }
        else if (root.TryGetProperty("SessionId", out var sessionIdElement2))
        {
            sessionIdElement2.GetString().Should().NotBeNullOrEmpty();
        }
        else
        {
            // If neither exists, let's see what properties are available
            var properties = root.EnumerateObject().Select(p => p.Name).ToList();
            Console.WriteLine($"Available properties: {string.Join(", ", properties)}");
            throw new Exception($"Neither 'sessionId' nor 'SessionId' found. Available: {string.Join(", ", properties)}");
        }
        
        if (root.TryGetProperty("status", out var statusElement))
        {
            statusElement.GetString().Should().Be("Created");
        }
        else if (root.TryGetProperty("Status", out var statusElement2))
        {
            statusElement2.GetString().Should().Be("Created");
        }
    }
    
    [Fact]
    [Trait("Category", "ContainerIntegration")]
    public async Task CreateSession_ShouldReturnUniqueIds()
    {
        // Arrange
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response1 = await _gameSessionClient.PostAsync("/sessions", content);
        var response2 = await _gameSessionClient.PostAsync("/sessions", content);
        
        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();
        
        var jsonDoc1 = JsonDocument.Parse(content1);
        var jsonDoc2 = JsonDocument.Parse(content2);
        
        var id1 = jsonDoc1.RootElement.GetProperty("sessionId").GetString();
        var id2 = jsonDoc2.RootElement.GetProperty("sessionId").GetString();
        
        id1.Should().NotBe(id2);
    }
} 