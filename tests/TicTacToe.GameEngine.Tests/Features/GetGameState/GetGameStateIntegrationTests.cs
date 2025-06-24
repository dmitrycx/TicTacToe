using Xunit;
using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Endpoints;
using TicTacToe.GameEngine.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Persistence;

namespace TicTacToe.GameEngine.Tests.Features.GetGameState;

[Trait("Category", "Integration")]
public class GetGameStateIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

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
    public async Task GET_Games_ShouldReturnGameState_WhenGameExists()
    {
        // Arrange
        var gameId = await CreateGameAsync();

        // Act
        var response = await _client.GetAsync($"/games/{gameId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GameStateResponse>(content, _jsonOptions);

        result.Should().NotBeNull();
        result!.Board.Should().NotBeNull();
        result.Status.Should().Be(GameStatus.InProgress.ToString());
        result.Winner.Should().BeNull();
    }

    [Fact]
    public async Task GET_Games_ShouldReturnWinningState_WhenGameIsWon()
    {
        // Arrange
        var gameId = await CreateGameAsync();
        
        // Make moves to create winning scenario
        var moves = new[]
        {
            new { Player = Player.X.ToString(), Row = 0, Column = 0 },
            new { Player = Player.O.ToString(), Row = 1, Column = 0 },
            new { Player = Player.X.ToString(), Row = 0, Column = 1 },
            new { Player = Player.O.ToString(), Row = 1, Column = 1 },
            new { Player = Player.X.ToString(), Row = 0, Column = 2 }
        };
        
        foreach (var move in moves)
        {
            await _client.PostAsync($"/games/{gameId}/move", 
                new StringContent(JsonSerializer.Serialize(move), Encoding.UTF8, "application/json"));
        }

        // Act
        var response = await _client.GetAsync($"/games/{gameId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GameStateResponse>(content, _jsonOptions);

        result.Should().NotBeNull();
        result!.Status.Should().Be(GameStatus.Win.ToString());
        result.Winner.Should().Be(Player.X.ToString());
    }

    [Fact]
    public async Task GET_Games_ShouldReturn404_WhenGameNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/games/{gameId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 