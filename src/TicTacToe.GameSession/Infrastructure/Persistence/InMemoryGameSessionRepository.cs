namespace TicTacToe.GameSession.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of the game session repository.
/// </summary>
public class InMemoryGameSessionRepository : IGameSessionRepository
{
    private readonly Dictionary<Guid, TicTacToe.GameSession.Domain.Aggregates.GameSession> _sessions = new();

    /// <summary>
    /// Retrieves a game session by its ID.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <returns>The game session, or null if not found.</returns>
    public Task<TicTacToe.GameSession.Domain.Aggregates.GameSession?> GetByIdAsync(Guid id)
    {
        _sessions.TryGetValue(id, out var session);
        return Task.FromResult(session);
    }

    /// <summary>
    /// Saves or updates a game session in the repository.
    /// </summary>
    /// <param name="session">The session to save or update.</param>
    /// <returns>The saved session instance.</returns>
    public Task<TicTacToe.GameSession.Domain.Aggregates.GameSession> SaveAsync(TicTacToe.GameSession.Domain.Aggregates.GameSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        // The indexer handles both add and update seamlessly
        _sessions[session.Id] = session;
        return Task.FromResult(session);
    }

    /// <summary>
    /// Deletes a game session by its ID.
    /// </summary>
    /// <param name="id">The session ID to delete.</param>
    /// <returns>True if the session was successfully deleted; otherwise, false.</returns>
    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _sessions.Remove(id);
        return Task.FromResult(removed);
    }

    /// <summary>
    /// Retrieves all game sessions.
    /// </summary>
    /// <returns>A collection of all game sessions.</returns>
    public Task<IEnumerable<TicTacToe.GameSession.Domain.Aggregates.GameSession>> GetAllAsync()
    {
        return Task.FromResult(_sessions.Values.AsEnumerable());
    }
} 