namespace TicTacToe.GameSession.Services;

/// <summary>
/// Service for sending real-time notifications via SignalR.
/// </summary>
public interface ISignalRNotificationService
{
    /// <summary>
    /// Notifies clients about a game state update.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="gameState">The updated game state.</param>
    Task NotifyGameStateUpdatedAsync(string sessionId, object gameState);

    /// <summary>
    /// Notifies clients about a move being made.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="move">The move that was made.</param>
    Task NotifyMoveReceivedAsync(string sessionId, object move);

    /// <summary>
    /// Notifies clients about game completion.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="finalState">The final game state.</param>
    Task NotifyGameCompletedAsync(string sessionId, object finalState);

    /// <summary>
    /// Notifies clients about an error.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="errorMessage">The error message.</param>
    Task NotifyErrorAsync(string sessionId, string errorMessage);
} 