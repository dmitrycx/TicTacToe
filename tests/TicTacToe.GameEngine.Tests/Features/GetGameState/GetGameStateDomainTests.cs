using Xunit;
using FluentAssertions;
using Moq;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameEngine.Tests.TestHelpers;
using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.Shared.Enums;

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
        var game = GameTestHelpers.CreateWinningGame(Player.X);
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
        var game = GameTestHelpers.CreateWinningGame(Player.X);

        // Act & Assert
        game.Id.Should().NotBeEmpty();
        game.Status.Should().Be(GameStatus.Win);
        game.CurrentPlayer.Should().Be(Player.X);
        game.Winner.Should().Be(Player.X);
        game.Board.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_Board_ShouldBeConvertibleToStrings()
    {
        // Arrange
        var game = GameTestHelpers.CreateWinningGame(Player.X);

        // Act
        var boardLists = game.Board.ToListOfLists();

        // Assert
        boardLists.Should().HaveCount(3);
        boardLists[0].Should().HaveCount(3);
        boardLists[1].Should().HaveCount(3);
        boardLists[2].Should().HaveCount(3);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_WinningGame_ShouldHaveCorrectState()
    {
        // Arrange & Act
        var game = GameTestHelpers.CreateWinningGame(Player.X);

        // Assert
        game.Status.Should().Be(GameStatus.Win);
        game.Winner.Should().Be(Player.X);
        game.CurrentPlayer.Should().Be(Player.X);
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
        game.CurrentPlayer.Should().Be(Player.X);
    }
} 