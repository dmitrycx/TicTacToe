using Xunit;
using FluentAssertions;
using Moq;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameEngine.Tests.TestHelpers;

namespace TicTacToe.GameEngine.Tests.Features.MakeMove;

/// <summary>
/// Unit tests for MakeMove domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class MakeMoveDomainTests
{
    [Fact]
    [Trait("Category", "Unit")]
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
    [Trait("Category", "Unit")]
    public async Task Repository_SaveAsync_ShouldPersistGame()
    {
        // Arrange
        var mockRepository = new Mock<IGameRepository>();
        mockRepository.Setup(r => r.SaveAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game game) => game);
        
        var game = GameTestHelpers.CreateNewGame();

        // Act
        var savedGame = await mockRepository.Object.SaveAsync(game);

        // Assert
        savedGame.Should().Be(game);
        mockRepository.Verify(r => r.SaveAsync(It.IsAny<Game>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_MakeMove_ShouldUpdateBoard_WhenValidMove()
    {
        // Arrange
        var game = GameTestHelpers.CreateNewGame();
        var position = Position.Create(0, 0);

        // Act
        game.MakeMove(position);

        // Assert
        game.Board.GetCell(position).Should().Be(Player.X);
        game.CurrentPlayer.Should().Be(Player.O);
        game.Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_MakeMove_ShouldThrowException_WhenPositionOccupied()
    {
        // Arrange
        var game = GameTestHelpers.CreateNewGame();
        var position = Position.Create(0, 0);
        game.MakeMove(position); // First move

        // Act & Assert
        var action = () => game.MakeMove(position);
        action.Should().Throw<InvalidMoveException>()
            .WithMessage("*Position is already occupied*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_MakeMove_ShouldResultInWin_WhenWinningMove()
    {
        // Arrange
        var game = GameTestHelpers.CreateGameWithWinningMoveAvailable();

        // Act
        game.MakeMove(Position.Create(2, 2));

        // Assert
        game.Status.Should().Be(GameStatus.Won);
        game.Winner.Should().Be(Player.X);
        game.CurrentPlayer.Should().Be(Player.O);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_MakeMove_ShouldResultInDraw_WhenDrawMove()
    {
        // Arrange
        var game = GameTestHelpers.CreateGameWithDrawMoveAvailable();

        // Act
        game.MakeMove(Position.Create(2, 2));

        // Assert
        game.Status.Should().Be(GameStatus.Draw);
        game.Winner.Should().BeNull();
        game.CurrentPlayer.Should().Be(Player.O);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Position_Create_ShouldValidateBounds()
    {
        // Act & Assert
        var validPosition = Position.Create(1, 1);
        validPosition.Row.Should().Be(1);
        validPosition.Column.Should().Be(1);

        var action = () => Position.Create(3, 1);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Row must be between 0 and 2*");

        action = () => Position.Create(1, 3);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Column must be between 0 and 2*");
    }
} 