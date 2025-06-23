namespace TicTacToe.GameSession.Infrastructure.Persistence;

/// <summary>
/// Repository interface for managing game sessions.
/// </summary>
public interface IGameSessionRepository
{
    /// <summary>
    /// Retrieves a game session by its ID.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <returns>The game session, or null if not found.</returns>
    Task<TicTacToe.GameSession.Domain.Aggregates.GameSession?> GetByIdAsync(Guid id);

    /// <summary>
    /// Saves or updates a game session in the repository.
    /// </summary>
    /// <param name="session">The session to save or update.</param>
    /// <returns>The saved session instance.</returns>
    Task<TicTacToe.GameSession.Domain.Aggregates.GameSession> SaveAsync(TicTacToe.GameSession.Domain.Aggregates.GameSession session);

    /// <summary>
    /// Deletes a game session by its ID.
    /// </summary>
    /// <param name="id">The session ID to delete.</param>
    /// <returns>True if the session was successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves all game sessions.
    /// </summary>
    /// <returns>A collection of all game sessions.</returns>
    Task<IEnumerable<TicTacToe.GameSession.Domain.Aggregates.GameSession>> GetAllAsync();
} 