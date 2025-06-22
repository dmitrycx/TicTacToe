namespace TicTacToe.GameSession.Domain.Events;

/// <summary>
/// Event raised when a move is made in a game session.
/// </summary>
public class MoveMadeEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    public Guid SessionId { get; }

    public Move Move { get; }

    /// <summary>
    /// Creates a new move made event.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="move">The move that was made.</param>
    public MoveMadeEvent(Guid sessionId, Move move)
    {
        SessionId = sessionId;
        Move = move;
        OccurredOn = DateTime.UtcNow;
    }
} 