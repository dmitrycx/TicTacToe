namespace TicTacToe.GameSession.Domain.Events;

/// <summary>
/// Event raised when game simulation starts.
/// </summary>
public class SimulationStartedEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    public Guid SessionId { get; }

    /// <summary>
    /// Creates a new simulation started event.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    public SimulationStartedEvent(Guid sessionId)
    {
        SessionId = sessionId;
        OccurredOn = DateTime.UtcNow;
    }
} 