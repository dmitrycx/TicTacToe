using TicTacToe.GameSession.UnitTests.TestHelpers;

namespace TicTacToe.GameSession.UnitTests.Domain.Entities;

public class MoveTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateMoveWithCorrectProperties()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        const Player player = Player.X;
        var position = new Position(1, 2);
        const MoveType moveType = MoveType.Random;
        const int moveNumber = 5;

        // Act
        var move = MoveFactory.CreateMove(sessionId, position.Row, position.Column, player, moveNumber);

        // Assert
        Assert.NotEqual(Guid.Empty, move.Id);
        Assert.Equal(sessionId, move.SessionId);
        Assert.Equal(player, move.Player);
        Assert.Equal(position, move.Position);
        Assert.Equal(moveType, move.Type);
        Assert.Equal(moveNumber, move.MoveNumber);
        Assert.NotNull(move.MadeAt);
    }

    [Fact]
    public void Constructor_ShouldSetMadeAtToCurrentTime()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var move = MoveFactory.CreateXMove(sessionId, 0, 0);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(move.MadeAt >= beforeCreation);
        Assert.True(move.MadeAt <= afterCreation);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(0, 2)]
    [InlineData(2, 0)]
    public void Constructor_WithValidPositions_ShouldCreateMove(int row, int column)
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var player = Player.O;

        // Act
        var move = MoveFactory.CreateMove(sessionId, row, column, player);

        // Assert
        Assert.Equal(row, move.Position.Row);
        Assert.Equal(column, move.Position.Column);
    }

    [Theory]
    [InlineData(Player.X)]
    [InlineData(Player.O)]
    public void Constructor_WithDifferentPlayers_ShouldCreateMove(Player player)
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var move = MoveFactory.CreateMove(sessionId, 0, 0, player);

        // Assert
        Assert.Equal(player, move.Player);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void Constructor_WithDifferentMoveNumbers_ShouldCreateMove(int moveNumber)
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var move = MoveFactory.CreateMove(sessionId, 0, 0, Player.X, moveNumber);

        // Assert
        Assert.Equal(moveNumber, move.MoveNumber);
    }

    [Fact]
    public void CreateXMove_ShouldCreateMoveForPlayerX()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var move = MoveFactory.CreateXMove(sessionId, 1, 1);

        // Assert
        Assert.Equal(Player.X, move.Player);
        Assert.Equal(1, move.Position.Row);
        Assert.Equal(1, move.Position.Column);
    }

    [Fact]
    public void CreateOMove_ShouldCreateMoveForPlayerO()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var move = MoveFactory.CreateOMove(sessionId, 2, 0);

        // Assert
        Assert.Equal(Player.O, move.Player);
        Assert.Equal(2, move.Position.Row);
        Assert.Equal(0, move.Position.Column);
    }

    [Fact]
    public void CreateMoveSequence_ShouldCreateCorrectNumberOfMoves()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        const int expectedCount = 5;

        // Act
        var moves = MoveFactory.CreateMoveSequence(sessionId, expectedCount);

        // Assert
        Assert.Equal(expectedCount, moves.Count);
        Assert.All(moves, move => Assert.Equal(sessionId, move.SessionId));
    }

    [Fact]
    public void CreateMoveSequence_ShouldAlternatePlayers()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        const int count = 4;

        // Act
        var moves = MoveFactory.CreateMoveSequence(sessionId, count);

        // Assert
        Assert.Equal(Player.X, moves[0].Player);
        Assert.Equal(Player.O, moves[1].Player);
        Assert.Equal(Player.X, moves[2].Player);
        Assert.Equal(Player.O, moves[3].Player);
    }

    [Fact]
    public void CreateMoveSequence_ShouldIncrementMoveNumbers()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        const int count = 3;

        // Act
        var moves = MoveFactory.CreateMoveSequence(sessionId, count);

        // Assert
        Assert.Equal(1, moves[0].MoveNumber);
        Assert.Equal(2, moves[1].MoveNumber);
        Assert.Equal(3, moves[2].MoveNumber);
    }
} 