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
    }
} 