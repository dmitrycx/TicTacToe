using Xunit;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Domain.Exceptions;
using System;

namespace TicTacToe.GameEngine.UnitTests.Domain.Aggregates;

public class GameTests
{
    [Fact]
    public void CreateGame_ShouldCreateNewGameWithEmptyBoard()
    {
        // Arrange & Act
        var game = Game.Create();

        // Assert
        Assert.NotNull(game);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.True(game.Board.IsEmpty());
    }

    [Fact]
    public void MakeMove_ValidMove_ShouldUpdateBoardAndSwitchPlayer()
    {
        // Arrange
        var game = Game.Create();
        var position = Position.Create(0, 0);

        // Act
        game.MakeMove(position);

        // Assert
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Equal(Player.X, game.Board.GetCell(position));
    }

    [Fact]
    public void MakeMove_InvalidPosition_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Position.Create(3, 3)); // Outside board
    }

    [Fact]
    public void MakeMove_OccupiedCell_ShouldThrowInvalidMoveException()
    {
        // Arrange
        var game = Game.Create();
        var position = Position.Create(0, 0);
        game.MakeMove(position); // X moves first

        // Act & Assert
        Assert.Throws<InvalidMoveException>(() => game.MakeMove(position));
    }

    [Fact]
    public void MakeMove_GameAlreadyEnded_ShouldThrowInvalidMoveException()
    {
        // Arrange
        var game = Game.Create();
        // Simulate a winning game for X
        game.MakeMove(Position.Create(0, 0)); // X
        game.MakeMove(Position.Create(1, 0)); // O
        game.MakeMove(Position.Create(0, 1)); // X
        game.MakeMove(Position.Create(1, 1)); // O
        game.MakeMove(Position.Create(0, 2)); // X wins

        // Act & Assert
        Assert.Throws<InvalidMoveException>(() => game.MakeMove(Position.Create(2, 2)));
    }

    [Fact]
    public void MakeMove_HorizontalWin_ShouldSetGameStatusToWin()
    {
        // Arrange
        var game = Game.Create();

        // Act - X wins horizontally
        game.MakeMove(Position.Create(0, 0)); // X
        game.MakeMove(Position.Create(1, 0)); // O
        game.MakeMove(Position.Create(0, 1)); // X
        game.MakeMove(Position.Create(1, 1)); // O
        game.MakeMove(Position.Create(0, 2)); // X wins

        // Assert
        Assert.Equal(GameStatus.Win, game.Status);
        Assert.Equal(Player.X, game.Winner);
    }

    [Fact]
    public void MakeMove_VerticalWin_ShouldSetGameStatusToWin()
    {
        // Arrange
        var game = Game.Create();

        // Act - X wins vertically
        game.MakeMove(Position.Create(0, 0)); // X
        game.MakeMove(Position.Create(0, 1)); // O
        game.MakeMove(Position.Create(1, 0)); // X
        game.MakeMove(Position.Create(1, 1)); // O
        game.MakeMove(Position.Create(2, 0)); // X wins

        // Assert
        Assert.Equal(GameStatus.Win, game.Status);
        Assert.Equal(Player.X, game.Winner);
    }

    [Fact]
    public void MakeMove_DiagonalWin_ShouldSetGameStatusToWin()
    {
        // Arrange
        var game = Game.Create();

        // Act - X wins diagonally
        game.MakeMove(Position.Create(0, 0)); // X
        game.MakeMove(Position.Create(0, 1)); // O
        game.MakeMove(Position.Create(1, 1)); // X
        game.MakeMove(Position.Create(0, 2)); // O
        game.MakeMove(Position.Create(2, 2)); // X wins

        // Assert
        Assert.Equal(GameStatus.Win, game.Status);
        Assert.Equal(Player.X, game.Winner);
    }

    [Fact]
   public void MakeMove_ResultsInDraw_ShouldSetGameStatusToDraw()
   {
       // Arrange
       var game = Game.Create();
   
       // Act - A sequence that guarantees a draw
       game.MakeMove(Position.Create(0, 0)); // X
       game.MakeMove(Position.Create(1, 1)); // O
       game.MakeMove(Position.Create(0, 1)); // X
       game.MakeMove(Position.Create(0, 2)); // O
       game.MakeMove(Position.Create(2, 0)); // X
       game.MakeMove(Position.Create(1, 0)); // O
       game.MakeMove(Position.Create(1, 2)); // X
       game.MakeMove(Position.Create(2, 2)); // O
       game.MakeMove(Position.Create(2, 1)); // X - Board is full, no winner
   
       // Assert
       Assert.Equal(GameStatus.Draw, game.Status);
       Assert.Null(game.Winner);
   }
} 