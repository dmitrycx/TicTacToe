namespace TicTacToe.GameSession.Domain.Exceptions;

/// <summary>
/// Exception thrown when a session is not found.
/// </summary>
public class SessionNotFoundException : Exception
{
    public Guid SessionId { get; }

    /// <summary>
    /// Creates a new session not found exception.
    /// </summary>
    /// <param name="sessionId">The session ID that was not found.</param>
    public SessionNotFoundException(Guid sessionId) 
        : base($"Session with ID {sessionId} was not found.")
    {
        SessionId = sessionId;
    }

    /// <summary>
    /// Creates a new session not found exception.
    /// </summary>
    /// <param name="sessionId">The session ID that was not found.</param>
    /// <param name="innerException">The inner exception.</param>
    public SessionNotFoundException(Guid sessionId, Exception innerException) 
        : base($"Session with ID {sessionId} was not found.", innerException)
    {
        SessionId = sessionId;
    }
} 