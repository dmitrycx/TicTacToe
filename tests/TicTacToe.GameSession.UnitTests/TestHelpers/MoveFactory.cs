namespace TicTacToe.GameSession.UnitTests.TestHelpers;

/// <summary>
/// Factory for creating test Move instances.
/// </summary>
public static class MoveFactory
{
    /// <summary>
    /// Creates a move for the specified session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="row">The row position.</param>
    /// <param name="column">The column position.</param>
    /// <param name="player">The player making the move.</param>
    /// <param name="moveNumber">The move number in sequence.</param>
    /// <returns>A new Move instance.</returns>
    public static Move CreateMove(Guid sessionId, int row, int column, Player player, int moveNumber = 1)
    {
        var position = new Position(row, column);
        return new Move(sessionId, player, position, MoveType.Random, moveNumber);
    }

    /// <summary>
    /// Creates a move for player X at the specified position.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="row">The row position.</param>
    /// <param name="column">The column position.</param>
    /// <param name="moveNumber">The move number in sequence.</param>
    /// <returns>A new Move instance for player X.</returns>
    public static Move CreateXMove(Guid sessionId, int row, int column, int moveNumber = 1)
    {
        return CreateMove(sessionId, row, column, Player.X, moveNumber);
    }

    /// <summary>
    /// Creates a move for player O at the specified position.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="row">The row position.</param>
    /// <param name="column">The column position.</param>
    /// <param name="moveNumber">The move number in sequence.</param>
    /// <returns>A new Move instance for player O.</returns>
    public static Move CreateOMove(Guid sessionId, int row, int column, int moveNumber = 1)
    {
        return CreateMove(sessionId, row, column, Player.O, moveNumber);
    }

    /// <summary>
    /// Creates a sequence of moves for testing.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="count">Number of moves to create.</param>
    /// <returns>A list of moves.</returns>
    public static List<Move> CreateMoveSequence(Guid sessionId, int count)
    {
        var moves = new List<Move>();
        
        for (var i = 0; i < count; i++)
        {
            var row = i % 3;
            var column = i / 3;
            var player = i % 2 == 0 ? Player.X : Player.O;
            
            moves.Add(CreateMove(sessionId, row, column, player, i + 1));
        }
        
        return moves;
    }
} 