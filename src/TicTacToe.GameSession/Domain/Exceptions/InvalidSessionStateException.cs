namespace TicTacToe.GameSession.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid session state transition is attempted.
/// </summary>
public class InvalidSessionStateException : Exception
{
    /// <summary>
    /// Creates a new invalid session state exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidSessionStateException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new invalid session state exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidSessionStateException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 