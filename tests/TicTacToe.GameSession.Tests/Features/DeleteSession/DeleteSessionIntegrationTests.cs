using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.DeleteSession;

[Trait("Category", "Integration")]
public class DeleteSessionIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task DELETE_Sessions_ShouldDeleteExistingSession_WhenValidRequest()
    {
        // Arrange - Create a session first
        var createResponse = await _client.PostAsync("/sessions", null);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);
        var sessionId = createResult!.SessionId;

        // Act - Delete the session
        var deleteResponse = await _client.DeleteAsync($"/sessions/{sessionId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var deleteContent = await deleteResponse.Content.ReadAsStringAsync();
        var deleteResult = JsonSerializer.Deserialize<DeleteSessionResponse>(deleteContent, _jsonOptions);
        
        deleteResult.Should().NotBeNull();
        deleteResult!.Success.Should().BeTrue();
        deleteResult.Message.Should().Contain(sessionId.ToString());
    }

    [Fact]
    public async Task DELETE_Sessions_ShouldReturnNotFound_WhenSessionDoesNotExist()
    {
        // Arrange
        var nonExistentSessionId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/sessions/{nonExistentSessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_Sessions_ShouldRemoveSessionFromRepository_WhenSuccessful()
    {
        // Arrange - Create a session first
        var createResponse = await _client.PostAsync("/sessions", null);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);
        var sessionId = createResult!.SessionId;

        // Verify session exists in repository
        var existingSession = await fixture.GameSessionRepository.GetByIdAsync(sessionId);
        existingSession.Should().NotBeNull();

        // Act - Delete the session
        var deleteResponse = await _client.DeleteAsync($"/sessions/{sessionId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert - Verify session is removed from repository
        var deletedSession = await fixture.GameSessionRepository.GetByIdAsync(sessionId);
        deletedSession.Should().BeNull();
    }

    [Fact]
    public async Task DELETE_Sessions_ShouldReturnCorrectResponseFormat_WhenSuccessful()
    {
        // Arrange - Create a session first
        var createResponse = await _client.PostAsync("/sessions", null);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);
        var sessionId = createResult!.SessionId;

        // Act
        var response = await _client.DeleteAsync($"/sessions/{sessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DeleteSessionResponse>(content, _jsonOptions);
        
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
        result.Message.Should().Contain("successfully deleted");
    }

    [Fact]
    public async Task DELETE_Sessions_ShouldHandleMultipleDeletions()
    {
        // Arrange - Create multiple sessions
        var sessionIds = new List<Guid>();
        
        for (var i = 0; i < 3; i++)
        {
            var createResponse = await _client.PostAsync("/sessions", null);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);
            sessionIds.Add(createResult!.SessionId);
        }

        // Act - Delete all sessions
        var deleteResponses = new List<HttpResponseMessage>();
        foreach (var sessionId in sessionIds)
        {
            var deleteResponse = await _client.DeleteAsync($"/sessions/{sessionId}");
            deleteResponses.Add(deleteResponse);
        }

        // Assert
        deleteResponses.Should().HaveCount(3);
        deleteResponses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
        
        // Verify all sessions are removed from repository
        foreach (var sessionId in sessionIds)
        {
            var deletedSession = await fixture.GameSessionRepository.GetByIdAsync(sessionId);
            deletedSession.Should().BeNull();
        }
    }
} 