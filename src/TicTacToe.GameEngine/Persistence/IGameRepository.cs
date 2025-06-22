using TicTacToe.GameEngine.Domain.Aggregates;

namespace TicTacToe.GameEngine.Persistence;

/// <summary>
/// Repository interface for managing Game aggregate persistence operations.
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// Retrieves a game by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the game to retrieve.</param>
    /// <returns>The game if found; otherwise, null.</returns>
    Task<Game?> GetByIdAsync(Guid id);

    /// <summary>
    /// Saves or updates a game in the repository.
    /// </summary>
    /// <param name="game">The game to save or update.</param>
    /// <returns>The saved game instance.</returns>
    Task<Game> SaveAsync(Game game);

    /// <summary>
    /// Deletes a game from the repository by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the game to delete.</param>
    /// <returns>True if the game was successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves all games from the repository.
    /// </summary>
    /// <returns>A collection of all games in the repository.</returns>
    Task<IEnumerable<Game>> GetAllAsync();
} 