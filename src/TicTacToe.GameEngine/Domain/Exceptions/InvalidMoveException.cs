namespace TicTacToe.GameEngine.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid move is attempted in a Tic Tac Toe game.
/// This exception is raised when a player tries to make a move that violates
/// the game rules, such as:
/// - Moving on an already occupied cell
/// - Moving outside the board boundaries
/// - Moving after the game has already ended (win or draw)
/// - Moving when it's not the player's turn
/// </summary>
public class InvalidMoveException : Exception
{
    /// <summary>
    /// Initializes a new instance of the InvalidMoveException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidMoveException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the InvalidMoveException class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InvalidMoveException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 