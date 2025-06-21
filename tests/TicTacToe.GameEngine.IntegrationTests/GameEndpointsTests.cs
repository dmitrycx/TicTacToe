using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToe.GameEngine.Endpoints.DTOs;
using Xunit;
using System.Net.Http.Json;

namespace TicTacToe.GameEngine.IntegrationTests.Endpoints;

public class GameEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GameEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task CreateGame_ShouldReturnCreatedGame()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateGameRequest();

        // Act
        var response = await client.PostAsJsonAsync("/games", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var gameResponse = JsonSerializer.Deserialize<CreateGameResponse>(content, _jsonOptions);
        
        Assert.NotNull(gameResponse);
        Assert.NotEqual(Guid.Empty, gameResponse.GameId);
    }

    [Fact]
    public async Task GetGameState_ExistingGame_ShouldReturnGameState()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Create a game first
        var createResponse = await client.PostAsJsonAsync("/games", new CreateGameRequest());
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<CreateGameResponse>(createContent, _jsonOptions);

        // Act
        var response = await client.GetAsync($"/games/{createResult!.GameId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var gameState = JsonSerializer.Deserialize<GameStateResponse>(content, _jsonOptions);
        
        Assert.NotNull(gameState);
        Assert.Equal(createResult.GameId, gameState.GameId);
    }

    [Fact]
    public async Task GetGameState_NonExistentGame_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/games/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MakeMove_ValidMove_ShouldUpdateGameState()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Create a game first
        var createResponse = await client.PostAsJsonAsync("/games", new CreateGameRequest());
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<CreateGameResponse>(createContent, _jsonOptions);

        var moveRequest = new MakeMoveRequest(0, 0);

        // Act
        var response = await client.PostAsJsonAsync($"/games/{createResult!.GameId}/move", moveRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var gameState = JsonSerializer.Deserialize<GameStateResponse>(content, _jsonOptions);
        
        Assert.NotNull(gameState);
        Assert.Equal(createResult.GameId, gameState.GameId);
    }

    [Fact]
    public async Task MakeMove_InvalidPosition_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Create a game first
        var createResponse = await client.PostAsJsonAsync("/games", new CreateGameRequest());
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<CreateGameResponse>(createContent, _jsonOptions);

        var invalidMoveRequest = new MakeMoveRequest(3, 3); // Outside board

        // Act
        var response = await client.PostAsJsonAsync($"/games/{createResult!.GameId}/move", invalidMoveRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
} 