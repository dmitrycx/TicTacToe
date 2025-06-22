namespace TicTacToe.GameSession.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of the game session repository.
/// </summary>
public class InMemoryGameSessionRepository : IGameSessionRepository
{
    private readonly Dictionary<Guid, TicTacToe.GameSession.Domain.Aggregates.GameSession> _sessions = new();

    /// <summary>
    /// Adds a new game session to the repository.
    /// </summary>
    /// <param name="session">The session to add.</param>
    public async Task AddAsync(TicTacToe.GameSession.Domain.Aggregates.GameSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        if (_sessions.ContainsKey(session.Id))
            throw new InvalidOperationException($"Session with ID {session.Id} already exists.");

        _sessions[session.Id] = session;
        await Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a game session by its ID.
    /// </summary>
    /// <param name="id">The session ID.</param>
    /// <returns>The game session, or null if not found.</returns>
    public async Task<TicTacToe.GameSession.Domain.Aggregates.GameSession?> GetByIdAsync(Guid id)
    {
        _sessions.TryGetValue(id, out var session);
        return await Task.FromResult(session);
    }

    /// <summary>
    /// Updates an existing game session.
    /// </summary>
    /// <param name="session">The session to update.</param>
    public async Task UpdateAsync(TicTacToe.GameSession.Domain.Aggregates.GameSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        if (!_sessions.ContainsKey(session.Id))
            throw new InvalidOperationException($"Session with ID {session.Id} does not exist.");

        _sessions[session.Id] = session;
        await Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves all game sessions.
    /// </summary>
    /// <returns>A collection of all game sessions.</returns>
    public async Task<IEnumerable<TicTacToe.GameSession.Domain.Aggregates.GameSession>> GetAllAsync()
    {
        return await Task.FromResult(_sessions.Values.AsEnumerable());
    }

    /// <summary>
    /// Deletes a game session.
    /// </summary>
    /// <param name="id">The session ID to delete.</param>
    public Task DeleteAsync(Guid id)
    {
        _sessions.Remove(id);
        return Task.CompletedTask;
    }
} 