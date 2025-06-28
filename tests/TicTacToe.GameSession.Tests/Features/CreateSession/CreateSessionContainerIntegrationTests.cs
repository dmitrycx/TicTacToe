using System.Net;
using System.Text;
using System.Text.Json;

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
        var session = JsonSerializer.Deserialize<dynamic>(responseContent, _jsonOptions);
        
        session.Should().NotBeNull();
        session.GetProperty("id").GetString().Should().NotBeNullOrEmpty();
        session.GetProperty("status").GetString().Should().Be("Created");
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
        
        var session1 = JsonSerializer.Deserialize<dynamic>(content1, _jsonOptions);
        var session2 = JsonSerializer.Deserialize<dynamic>(content2, _jsonOptions);
        
        var id1 = session1.GetProperty("id").GetString();
        var id2 = session2.GetProperty("id").GetString();
        
        id1.Should().NotBe(id2);
    }
} 