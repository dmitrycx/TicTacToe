namespace TicTacToe.GameSession.Infrastructure.Persistence;

/// <summary>
/// Repository interface for managing game sessions.
/// </summary>
public interface IGameSessionRepository
{
    /// <summary>
    /// Adds a new game session.
    /// </summary>
    /// <param name="session">The session to add.</param>
    Task AddAsync(TicTacToe.GameSession.Domain.Aggregates.GameSession session);

    /// <summary>
    /// Retrieves a game session by its ID.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <returns>The game session, or null if not found.</returns>
    Task<TicTacToe.GameSession.Domain.Aggregates.GameSession?> GetByIdAsync(Guid id);

    /// <summary>
    /// Updates an existing game session.
    /// </summary>
    /// <param name="session">The session to update.</param>
    Task UpdateAsync(TicTacToe.GameSession.Domain.Aggregates.GameSession session);

    /// <summary>
    /// Retrieves all game sessions.
    /// </summary>
    /// <returns>A collection of all game sessions.</returns>
    Task<IEnumerable<TicTacToe.GameSession.Domain.Aggregates.GameSession>> GetAllAsync();
} 