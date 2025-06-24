using Xunit;
using FluentAssertions;
using Moq;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameEngine.Tests.TestHelpers;

namespace TicTacToe.GameEngine.Tests.Features.GetGameState;

/// <summary>
/// Unit tests for GetGameState domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class GetGameStateDomainTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetByIdAsync_ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        var game = GameTestHelpers.CreateWinningGame();
        var mockRepository = new Mock<IGameRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        // Act
        var retrievedGame = await mockRepository.Object.GetByIdAsync(game.Id);

        // Assert
        retrievedGame.Should().Be(game);
        mockRepository.Verify(r => r.GetByIdAsync(game.Id), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetByIdAsync_ShouldReturnNull_WhenGameDoesNotExist()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var mockRepository = new Mock<IGameRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(gameId))
            .ReturnsAsync((Game?)null);

        // Act
        var retrievedGame = await mockRepository.Object.GetByIdAsync(gameId);

        // Assert
        retrievedGame.Should().BeNull();
        mockRepository.Verify(r => r.GetByIdAsync(gameId), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_Properties_ShouldBeAccessible()
    {
        // Arrange
        var game = GameTestHelpers.CreateWinningGame();

        // Act & Assert
        game.Id.Should().NotBeEmpty();
        game.Status.Should().Be(GameStatus.Won);
        game.CurrentPlayer.Should().Be(Player.O);
        game.Winner.Should().Be(Player.X);
        game.Board.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_Board_ShouldBeConvertibleToStrings()
    {
        // Arrange
        var game = GameTestHelpers.CreateWinningGame();

        // Act
        var boardStrings = game.Board.ToStrings();

        // Assert
        boardStrings.Should().HaveCount(3);
        boardStrings[0].Should().Be("X|O|X");
        boardStrings[1].Should().Be("O|X|O");
        boardStrings[2].Should().Be("X| | ");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_WinningGame_ShouldHaveCorrectState()
    {
        // Arrange & Act
        var game = GameTestHelpers.CreateWinningGame();

        // Assert
        game.Status.Should().Be(GameStatus.Won);
        game.Winner.Should().Be(Player.X);
        game.CurrentPlayer.Should().Be(Player.O);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_DrawGame_ShouldHaveCorrectState()
    {
        // Arrange & Act
        var game = GameTestHelpers.CreateDrawGame();

        // Assert
        game.Status.Should().Be(GameStatus.Draw);
        game.Winner.Should().BeNull();
        game.CurrentPlayer.Should().Be(Player.O);
    }
} 