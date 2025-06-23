using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameSession.Domain.Services;

namespace TicTacToe.GameSession.UnitTests.Domain.Services;

public class RandomMoveGeneratorTests
{
    private readonly RandomMoveGenerator _generator = new();

    [Fact]
    public void Type_ShouldReturnRandom()
    {
        // Act
        var moveType = _generator.Type;

        // Assert
        Assert.Equal(MoveType.Random, moveType);
    }

    [Fact]
    public void GenerateMove_EmptyBoard_ShouldReturnValidPosition()
    {
        // Arrange
        var board = new Board();

        // Act
        var position = _generator.GenerateMove(Player.X, board);

        // Assert
        Assert.NotNull(position);
        Assert.True(position.Row >= 0 && position.Row < 3);
        Assert.True(position.Column >= 0 && position.Column < 3);
        Assert.True(board.IsCellEmpty(position));
    }

    [Fact]
    public void GenerateMove_PartiallyFilledBoard_ShouldReturnEmptyPosition()
    {
        // Arrange
        var board = new Board();
        // Fill some positions
        board.SetCell(new Position(0, 0), Player.X);
        board.SetCell(new Position(1, 1), Player.O);
        board.SetCell(new Position(2, 2), Player.X);

        // Act
        var position = _generator.GenerateMove(Player.O, board);

        // Assert
        Assert.NotNull(position);
        Assert.True(board.IsCellEmpty(position));
        Assert.False(position.Equals(new Position(0, 0)));
        Assert.False(position.Equals(new Position(1, 1)));
        Assert.False(position.Equals(new Position(2, 2)));
    }

    [Fact]
    public void GenerateMove_FullBoard_ShouldThrowException()
    {
        // Arrange
        var board = new Board();
        // Fill all positions
        for (var row = 0; row < 3; row++)
        {
            for (var column = 0; column < 3; column++)
            {
                board.SetCell(new Position(row, column), row % 2 == 0 ? Player.X : Player.O);
            }
        }

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _generator.GenerateMove(Player.X, board));
        
        Assert.Contains("No valid moves available", exception.Message);
    }

    [Fact]
    public void GenerateMove_MultipleCalls_ShouldReturnDifferentPositions()
    {
        // Arrange
        var board = new Board();
        var positions = new HashSet<Position>();

        // Act
        for (var i = 0; i < 9; i++)
        {
            var position = _generator.GenerateMove(Player.X, board);
            positions.Add(position);
            board.SetCell(position, Player.X);
        }

        // Assert
        Assert.Equal(9, positions.Count); // All positions should be different
    }

    [Fact]
    public void GenerateMove_WithPlayerParameter_ShouldNotAffectGeneration()
    {
        // Arrange
        var board = new Board();

        // Act
        var positionX = _generator.GenerateMove(Player.X, board);
        var positionO = _generator.GenerateMove(Player.O, board);

        // Assert
        // Both should be valid positions (though they might be the same due to randomness)
        Assert.True(board.IsCellEmpty(positionX));
        Assert.True(board.IsCellEmpty(positionO));
    }
} 