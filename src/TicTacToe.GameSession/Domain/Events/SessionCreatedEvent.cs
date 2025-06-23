namespace TicTacToe.GameSession.Domain.Events;

/// <summary>
/// Event raised when a new game session is created.
/// </summary>
public class SessionCreatedEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    public Guid SessionId { get; }

    public Guid GameId { get; }

    /// <summary>
    /// Creates a new session created event.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="gameId">The game ID.</param>
    public SessionCreatedEvent(Guid sessionId, Guid gameId)
    {
        SessionId = sessionId;
        GameId = gameId;
        OccurredOn = DateTime.UtcNow;
    }
} 