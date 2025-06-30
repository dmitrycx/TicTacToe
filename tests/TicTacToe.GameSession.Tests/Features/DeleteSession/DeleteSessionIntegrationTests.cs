using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;
using TicTacToe.Shared.Enums;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace TicTacToe.GameSession.Tests.Features.DeleteSession;

[Trait("Category", "Integration")]
public class DeleteSessionIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task DELETE_Sessions_ShouldDeleteExistingSession_WhenValidRequest()
    {
        // Arrange - Create a session first
        var createRequest = new { strategy = GameStrategy.Random };
        var createResponse = await _client.PostAsync("/sessions", JsonContent.Create(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/sessions/{sessionResponse!.SessionId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DELETE_Sessions_ShouldReturn404_WhenSessionDoesNotExist()
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
        var createRequest = new { strategy = GameStrategy.Random };
        var createResponse = await _client.PostAsync("/sessions", JsonContent.Create(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);

        // Verify session exists before deletion
        var repository = fixture.GameSessionRepository;
        var sessionBeforeDelete = await repository.GetByIdAsync(sessionResponse!.SessionId);
        sessionBeforeDelete.Should().NotBeNull();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/sessions/{sessionResponse.SessionId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify session is removed from repository
        var sessionAfterDelete = await repository.GetByIdAsync(sessionResponse.SessionId);
        sessionAfterDelete.Should().BeNull();
    }

    [Fact]
    public async Task DELETE_Sessions_ShouldReturnCorrectResponseFormat_WhenSuccessful()
    {
        // Arrange - Create a session first
        var createRequest = new { strategy = GameStrategy.Random };
        var createResponse = await _client.PostAsync("/sessions", JsonContent.Create(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/sessions/{sessionResponse!.SessionId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        deleteResponse.Content.Headers.ContentLength.Should().Be(0);
    }

    [Fact]
    public async Task DELETE_Sessions_ShouldHandleMultipleDeletions()
    {
        // Arrange - Create multiple sessions
        var createRequest = new { strategy = GameStrategy.Random };
        var createResponse1 = await _client.PostAsync("/sessions", JsonContent.Create(createRequest));
        var createResponse2 = await _client.PostAsync("/sessions", JsonContent.Create(createRequest));
        
        var content1 = await createResponse1.Content.ReadAsStringAsync();
        var content2 = await createResponse2.Content.ReadAsStringAsync();
        
        var session1 = JsonSerializer.Deserialize<CreateSessionResponse>(content1, _jsonOptions);
        var session2 = JsonSerializer.Deserialize<CreateSessionResponse>(content2, _jsonOptions);

        // Act - Delete both sessions
        var deleteResponse1 = await _client.DeleteAsync($"/sessions/{session1!.SessionId}");
        var deleteResponse2 = await _client.DeleteAsync($"/sessions/{session2!.SessionId}");

        // Assert
        deleteResponse1.StatusCode.Should().Be(HttpStatusCode.NoContent);
        deleteResponse2.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify both sessions are removed from repository
        var repository = fixture.GameSessionRepository;
        var sessionAfterDelete1 = await repository.GetByIdAsync(session1.SessionId);
        var sessionAfterDelete2 = await repository.GetByIdAsync(session2.SessionId);
        
        sessionAfterDelete1.Should().BeNull();
        sessionAfterDelete2.Should().BeNull();
    }
} 