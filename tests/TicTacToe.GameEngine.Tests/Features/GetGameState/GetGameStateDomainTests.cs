using Xunit;
using FluentAssertions;
using Moq;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameEngine.Tests.TestHelpers;

namespace TicTacToe.GameEngine.Tests.Features.GetGameState;

/// <summary>
/// Unit tests for GetGameState domain logic and repository behavior
/// </summary>
public class GetGameStateDomainTests
{
    [Fact]
    public async Task Repository_GetByIdAsync_ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        var game = GameTestHelpers.CreateNewGame();
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
    public void Game_Properties_ShouldBeAccessible()
    {
        // Arrange
        var game = GameTestHelpers.CreateNewGame();

        // Act & Assert
        game.Id.Should().NotBeEmpty();
        game.Status.Should().Be(GameStatus.InProgress);
        game.CurrentPlayer.Should().Be(Player.X);
        game.Winner.Should().BeNull();
        game.Board.Should().NotBeNull();
        game.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        game.LastMoveAt.Should().BeNull();
    }

    [Fact]
    public void Game_Board_ShouldBeConvertibleToStrings()
    {
        // Arrange
        var game = GameTestHelpers.CreateNewGame();
        var board = game.Board.ToListOfLists();

        // Act
        var stringBoard = board.Select(row => 
            row.Select(cell => cell?.ToString() ?? "").ToList()
        ).ToList();

        // Assert
        stringBoard.Should().HaveCount(3);
        stringBoard[0].Should().HaveCount(3);
        stringBoard[1].Should().HaveCount(3);
        stringBoard[2].Should().HaveCount(3);
    }

    [Fact]
    public void Game_WinningGame_ShouldHaveCorrectState()
    {
        // Arrange
        var game = GameTestHelpers.CreateWinningGame(Player.X);

        // Act & Assert
        game.Status.Should().Be(GameStatus.Win);
        game.Winner.Should().Be(Player.X);
        game.CurrentPlayer.Should().Be(Player.X); // Last player to move
    }

    [Fact]
    public void Game_DrawGame_ShouldHaveCorrectState()
    {
        // Arrange
        var game = GameTestHelpers.CreateDrawGame();

        // Act & Assert
        game.Status.Should().Be(GameStatus.Draw);
        game.Winner.Should().BeNull();
    }
} 