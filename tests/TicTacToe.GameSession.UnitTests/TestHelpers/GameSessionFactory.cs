namespace TicTacToe.GameSession.UnitTests.TestHelpers;

/// <summary>
/// Factory for creating test GameSession instances with different states.
/// </summary>
public static class GameSessionFactory
{
    /// <summary>
    /// Creates a new game session in Created state.
    /// </summary>
    /// <returns>A new GameSession in Created state.</returns>
    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateCreatedSession()
    {
        return TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();
    }

    /// <summary>
    /// Creates a new game session in Created state with a specific game ID.
    /// </summary>
    /// <param name="gameId">The game ID to assign.</param>
    /// <returns>A new GameSession in Created state with the specified game ID.</returns>
    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateCreatedSession(Guid gameId)
    {
        var session = new TicTacToe.GameSession.Domain.Aggregates.GameSession(gameId);
        return session;
    }

    /// <summary>
    /// Creates a new game session in InProgress state.
    /// </summary>
    /// <returns>A new GameSession in InProgress state.</returns>
    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateInProgressSession()
    {
        var session = CreateCreatedSession();
        session.StartSimulation();
        return session;
    }

    /// <summary>
    /// Creates a new game session in Completed state.
    /// </summary>
    /// <param name="winner">The winner of the game (optional).</param>
    /// <returns>A new GameSession in Completed state.</returns>
    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateCompletedSession(string? winner = "X")
    {
        var session = CreateInProgressSession();
        session.CompleteGame(winner);
        return session;
    }

    /// <summary>
    /// Creates a new game session in Failed state.
    /// </summary>
    /// <returns>A new GameSession in Failed state.</returns>
    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateFailedSession()
    {
        var session = CreateInProgressSession();
        session.FailSimulation();
        return session;
    }

    /// <summary>
    /// Creates a game session with some moves already made.
    /// </summary>
    /// <param name="moveCount">Number of moves to add.</param>
    /// <returns>A GameSession with the specified number of moves.</returns>
    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateSessionWithMoves(int moveCount)
    {
        var session = CreateInProgressSession();
        
        for (var i = 0; i < moveCount; i++)
        {
            var row = i % 3;
            var column = i / 3;
            var position = new Position(row, column);
            var player = i % 2 == 0 ? Player.X : Player.O;
            
            session.RecordMove(position, player);
        }
        
        return session;
    }
} 