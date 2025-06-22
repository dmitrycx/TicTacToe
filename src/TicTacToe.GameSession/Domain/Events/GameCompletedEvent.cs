using TicTacToe.GameEngine.Domain.Enums;

namespace TicTacToe.GameSession.Domain.Events;

/// <summary>
/// Domain event raised when a game session is completed.
/// </summary>
public class GameCompletedEvent : IDomainEvent
{
    public Guid SessionId { get; }

    public GameStatus Result { get; }

    public DateTime OccurredOn { get; }

    /// <summary>
    /// Creates a new game completed event.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="result">The game result.</param>
    public GameCompletedEvent(Guid sessionId, GameStatus result)
    {
        SessionId = sessionId;
        Result = result;
        OccurredOn = DateTime.UtcNow;
    }
} 