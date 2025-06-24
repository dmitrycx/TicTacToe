using Xunit;
using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Endpoints;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameEngine.Tests.Fixtures;
using TicTacToe.GameEngine.Tests.TestHelpers;

namespace TicTacToe.GameEngine.Tests.Features.MakeMove;

public class MakeMoveIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly IGameRepository _gameRepository = fixture.GameRepository;

    // Helper method to create a game and return its ID
    private async Task<Guid> CreateGameAsync()
    {
        var response = await _client.PostAsync("/games",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateGameResponse>(content, _jsonOptions);
        return result!.GameId;
    }

    [Fact]
    public async Task POST_Games_Move_ShouldMakeValidMove_WhenGameExists()
    {
        // Arrange
        var gameId = await CreateGameAsync();
        var moveRequest = new { Row = 0, Column = 0 };

        // Act
        var response = await _client.PostAsync($"/games/{gameId}/move",
            new StringContent(JsonSerializer.Serialize(moveRequest), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GameStateResponse>(content, _jsonOptions);

        result.Should().NotBeNull();
        result!.Status.Should().Be(GameStatus.InProgress.ToString());
    }

    [Fact]
    public async Task POST_Games_Move_ShouldReturnWinningStatus_WhenMoveResultsInWin()
    {
        // ARRANGE: Create the game state directly in the repository.
        var game = GameTestHelpers.CreateGameWithMoves(
            (Player.X, 0, 0), (Player.O, 1, 0),
            (Player.X, 0, 1), (Player.O, 1, 1)
        );
        await _gameRepository.SaveAsync(game); // Save it directly to the in-memory store
        var gameId = game.Id;

        var winningMove = new { Row = 0, Column = 2 };

        // ACT: Call only the endpoint under test.
        var response = await _client.PostAsync($"/games/{gameId}/move",
            new StringContent(JsonSerializer.Serialize(winningMove), Encoding.UTF8, "application/json"));

        // ASSERT:
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GameStateResponse>(content, _jsonOptions);
        result.Should().NotBeNull();
        result!.Status.Should().Be(GameStatus.Win.ToString());
        result.Winner.Should().Be(Player.X.ToString());
    }

    [Fact]
    public async Task POST_Games_Move_ShouldReturn404_WhenGameNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var moveRequest = new { Row = 0, Column = 0 };

        // Act
        var response = await _client.PostAsync($"/games/{gameId}/move",
            new StringContent(JsonSerializer.Serialize(moveRequest), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 