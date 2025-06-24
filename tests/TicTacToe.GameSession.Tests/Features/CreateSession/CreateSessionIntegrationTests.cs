using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.CreateSession;

[Trait("Category", "Integration")]
public class CreateSessionIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    [Trait("Category", "Integration")]
    public async Task POST_Sessions_ShouldCreateNewSession_WhenValidRequest()
    {
        // Arrange
        var request = new { };

        // Act
        var response = await _client.PostAsync("/sessions",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateSessionResponse>(content, _jsonOptions);
        
        result.Should().NotBeNull();
        result!.SessionId.Should().NotBeEmpty();
        result.Status.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task POST_Sessions_ShouldReturnUniqueSessionIds_WhenMultipleRequests()
    {
        // Arrange
        var request = new { };

        // Act
        var response1 = await _client.PostAsync("/sessions",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        var response2 = await _client.PostAsync("/sessions",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();
        
        var result1 = JsonSerializer.Deserialize<CreateSessionResponse>(content1, _jsonOptions);
        var result2 = JsonSerializer.Deserialize<CreateSessionResponse>(content2, _jsonOptions);
        
        result1!.SessionId.Should().NotBe(result2!.SessionId);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task POST_Sessions_ShouldPersistSessionInRepository()
    {
        // Arrange
        var request = new { };

        // Act
        var response = await _client.PostAsync("/sessions",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateSessionResponse>(content, _jsonOptions);
        
        // Verify the session was actually persisted
        var persistedSession = await fixture.GameSessionRepository.GetByIdAsync(result!.SessionId);
        persistedSession.Should().NotBeNull();
        persistedSession!.Id.Should().Be(result.SessionId);
        persistedSession.Status.ToString().Should().Be(result.Status);
    }
} 