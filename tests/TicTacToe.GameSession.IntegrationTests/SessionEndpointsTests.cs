using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TicTacToe.GameSession.Infrastructure.External;
using TicTacToe.GameSession.Infrastructure.External.DTOs;
using TicTacToe.GameSession.Endpoints.DTOs;
using TicTacToe.GameSession.Domain.Constants;
using TicTacToe.GameSession.Infrastructure.Persistence;
using System.Text.Json;

namespace TicTacToe.GameSession.IntegrationTests;

public class SessionEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IGameEngineApiClient> _mockGameEngine;
    private readonly HttpClient _client;

    public SessionEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _mockGameEngine = new Mock<IGameEngineApiClient>();
        
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove all existing IGameEngineApiClient registrations
                services.RemoveAll<IGameEngineApiClient>();
                // Add the mock implementation as a singleton
                services.AddSingleton(_mockGameEngine.Object);
                
                // Ensure the repository is properly configured as a singleton
                services.RemoveAll<IGameSessionRepository>();
                services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetSession_NonExistentSession_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/sessions/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SimulateGame_ValidSession_ShouldCompleteSimulation()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        
        // Mock CreateGameAsync to return a valid game ID
        _mockGameEngine.Setup(x => x.CreateGameAsync())
            .ReturnsAsync(new CreateGameResponse(gameId, DateTime.UtcNow));
        
        // Mock GetGameStateAsync to return initial game state
        _mockGameEngine.Setup(x => x.GetGameStateAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new GameStateResponse(
                gameId,
                SessionConstants.Status.InProgress,
                "X",
                null,
                [
                    new List<string?> { null, null, null },
                    new List<string?> { null, null, null },
                    new List<string?> { null, null, null }
                ],
                DateTime.UtcNow,
                null
            ));
        
        // Mock MakeMoveAsync to return a completed game state
        _mockGameEngine.Setup(x => x.MakeMoveAsync(It.IsAny<Guid>(), It.IsAny<MakeMoveRequest>()))
            .ReturnsAsync(new GameStateResponse(
                gameId,
                SessionConstants.Status.Completed,
                "X",
                "X",
                [
                    new List<string?> { "X", "O", "X" },
                    new List<string?> { "O", "X", "O" },
                    new List<string?> { "X", null, null }
                ],
                DateTime.UtcNow,
                DateTime.UtcNow
            ));
        
        // Create a session first
        var createResponse = await _client.PostAsync("/sessions", null);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();

        // Act
        var response = await _client.PostAsJsonAsync($"/sessions/{createResult!.SessionId}/simulate", new { });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var simulationResult = await response.Content.ReadFromJsonAsync<SimulateGameResponse>();
        
        Assert.NotNull(simulationResult);
        Assert.Equal(createResult.SessionId, simulationResult.SessionId);
        Assert.True(simulationResult.IsCompleted);
        Assert.NotNull(simulationResult.Winner);
        Assert.True(simulationResult.Moves.Count > 0);
    }

    [Fact]
    public async Task SimulateGame_NonExistentSession_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsJsonAsync($"/sessions/{nonExistentId}/simulate", new { });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SimulateGame_AlreadyCompletedSession_ShouldReturnBadRequest()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        
        // Mock CreateGameAsync to return a valid game ID
        _mockGameEngine.Setup(x => x.CreateGameAsync())
            .ReturnsAsync(new CreateGameResponse(gameId, DateTime.UtcNow));
        
        // Mock GetGameStateAsync to return initial game state
        _mockGameEngine.Setup(x => x.GetGameStateAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new GameStateResponse(
                gameId,
                SessionConstants.Status.InProgress,
                "X",
                null,
                [
                    new List<string?> { null, null, null },
                    new List<string?> { null, null, null },
                    new List<string?> { null, null, null }
                ],
                DateTime.UtcNow,
                null
            ));
        
        // Simplified mock: return a completed game state immediately
        // This focuses the test on the "already completed" scenario without
        // depending on internal simulation loop implementation details
        _mockGameEngine.Setup(x => x.MakeMoveAsync(It.IsAny<Guid>(), It.IsAny<MakeMoveRequest>()))
            .ReturnsAsync(new GameStateResponse(
                gameId,
                SessionConstants.Status.Completed,
                "X",
                "X",
                new List<List<string?>>
                {
                    new List<string?> { "X", "O", "X" },
                    new List<string?> { "O", "X", "O" },
                    new List<string?> { "X", null, null }
                },
                DateTime.UtcNow,
                DateTime.UtcNow
            ));
        
        // Create and simulate a session (this should complete successfully)
        var createResponse = await _client.PostAsync("/sessions", null);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        
        await _client.PostAsJsonAsync($"/sessions/{createResult!.SessionId}/simulate", new { });

        // Act - Try to simulate again (this should fail because session is already completed)
        var response = await _client.PostAsJsonAsync($"/sessions/{createResult.SessionId}/simulate", new { });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        // For error responses, we might want to read the content as a string to verify the error message
        // This is acceptable since we're testing error content, not structured JSON
        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains(SessionConstants.ErrorMessages.SessionNotInCreatedState, errorContent);
    }

    [Fact]
    public async Task CreateSession_WhenCalled_ReturnsCreatedWithSessionDetails()
    {
        // Arrange

        // Act
        var response = await _client.PostAsync("/sessions", null);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createResult = await response.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createResult);
        Assert.NotEqual(Guid.Empty, createResult.SessionId);
        Assert.Equal(SessionConstants.Status.Created, createResult.Status);
    }

    [Fact]
    public async Task CreateAndGetSession_ShouldWorkCorrectly()
    {
        // Arrange & Act - Create session
        var createResponse = await _client.PostAsync("/sessions", null);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createResult);
        
        // Act - Get the created session
        var getResponse = await _client.GetAsync($"/sessions/{createResult.SessionId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        
        var getResult = await getResponse.Content.ReadFromJsonAsync<GetSessionResponse>();
        Assert.NotNull(getResult);
        Assert.Equal(createResult.SessionId, getResult.SessionId);
        Assert.Equal(SessionConstants.Status.Created, getResult.Status);
        Assert.Equal(Guid.Empty, getResult.GameId); // Game ID should be empty initially
        Assert.NotNull(getResult.CreatedAt);
        Assert.Null(getResult.StartedAt);
        Assert.Null(getResult.CompletedAt);
        Assert.Empty(getResult.Moves);
        Assert.Null(getResult.Winner);
        Assert.Null(getResult.Result);
    }

    [Fact]
    public async Task ListSessions_WhenCalled_ReturnsAllSessions()
    {
        // Arrange - Create multiple sessions
        var createResponse1 = await _client.PostAsync("/sessions", null);
        var createResponse2 = await _client.PostAsync("/sessions", null);
        
        Assert.Equal(HttpStatusCode.Created, createResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createResponse2.StatusCode);

        // Get the session IDs
        var createResult1 = await createResponse1.Content.ReadFromJsonAsync<CreateSessionResponse>();
        var createResult2 = await createResponse2.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createResult1);
        Assert.NotNull(createResult2);

        // Act
        var response = await _client.GetAsync("/sessions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Use System.Text.Json syntax for property access
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(jsonDocument);
        
        var root = jsonDocument!.RootElement;
        Assert.True(root.TryGetProperty("sessions", out var sessionsArray));
        Assert.True(sessionsArray.GetArrayLength() >= 2);
        
        // Verify our created sessions are in the list
        var sessionIds = new List<Guid>();
        foreach (var session in sessionsArray.EnumerateArray())
        {
            if (session.TryGetProperty("sessionId", out var sessionIdElement))
            {
                sessionIds.Add(sessionIdElement.GetGuid());
            }
        }
        
        Assert.Contains(createResult1!.SessionId, sessionIds);
        Assert.Contains(createResult2!.SessionId, sessionIds);
    }

    [Fact]
    public async Task DeleteSession_ExistingSession_ShouldReturnSuccess()
    {
        // Arrange - Create a session
        var createResponse = await _client.PostAsync("/sessions", null);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createResult);

        // Act
        var response = await _client.DeleteAsync($"/sessions/{createResult.SessionId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Use System.Text.Json syntax for property access
        var jsonDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(jsonDocument);
        
        var root = jsonDocument!.RootElement;
        Assert.True(root.TryGetProperty("success", out var successElement));
        Assert.True(successElement.GetBoolean());
        
        Assert.True(root.TryGetProperty("message", out var messageElement));
        var message = messageElement.GetString();
        Assert.Contains(createResult.SessionId.ToString(), message);
        
        // Verify session is actually deleted
        var getResponse = await _client.GetAsync($"/sessions/{createResult.SessionId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteSession_NonExistentSession_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/sessions/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
} 