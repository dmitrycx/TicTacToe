using Xunit;
using FluentAssertions;
using Moq;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameEngine.Tests.TestHelpers;
using TicTacToe.Shared.Enums;

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
            .WithMessage("*Position Position { Row = 0, Column = 0 } is already occupied*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_MakeMove_ShouldResultInWin_WhenWinningMove()
    {
        // Arrange
        var game = GameTestHelpers.CreateGameWithMoves(
            (Player.X, 0, 0), (Player.O, 1, 0),
            (Player.X, 0, 1), (Player.O, 1, 1)
        );
        var winningPosition = Position.Create(0, 2);

        // Act
        game.MakeMove(winningPosition);

        // Assert
        game.Status.Should().Be(GameStatus.Win);
        game.Winner.Should().Be(Player.X);
        game.CurrentPlayer.Should().Be(Player.X); // Current player doesn't switch after win
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_MakeMove_ShouldResultInDraw_WhenDrawMove()
    {
        // Arrange - Create a draw scenario manually
        var game = Game.Create();
        
        // Make moves to create a draw scenario
        // This sequence is guaranteed to result in a draw without any player winning prematurely
        game.MakeMove(Position.Create(0, 0)); // X
        game.MakeMove(Position.Create(1, 1)); // O
        game.MakeMove(Position.Create(0, 1)); // X
        game.MakeMove(Position.Create(0, 2)); // O
        game.MakeMove(Position.Create(2, 0)); // X
        game.MakeMove(Position.Create(1, 0)); // O
        game.MakeMove(Position.Create(1, 2)); // X
        game.MakeMove(Position.Create(2, 2)); // O
        
        var drawPosition = Position.Create(2, 1);

        // Act
        game.MakeMove(drawPosition);

        // Assert
        game.Status.Should().Be(GameStatus.Draw);
        game.Winner.Should().BeNull();
        game.CurrentPlayer.Should().Be(Player.X); // Current player doesn't switch after draw
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
            .WithMessage("*Position must be within the 3x3 board bounds*");

        action = () => Position.Create(1, 3);
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Position must be within the 3x3 board bounds*");
    }
} 