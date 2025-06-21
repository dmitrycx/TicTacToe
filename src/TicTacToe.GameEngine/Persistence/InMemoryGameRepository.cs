using TicTacToe.GameEngine.Domain.Aggregates;

namespace TicTacToe.GameEngine.Persistence;

public class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<Guid, Game> _games = new();

    public Task<Game?> GetByIdAsync(Guid id)
    {
        _games.TryGetValue(id, out var game);
        return Task.FromResult(game);
    }

    public Task<Game> SaveAsync(Game game)
    {
        _games[game.Id] = game;
        return Task.FromResult(game);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _games.Remove(id);
        return Task.FromResult(removed);
    }

    public Task<IEnumerable<Game>> GetAllAsync()
    {
        return Task.FromResult(_games.Values.AsEnumerable());
    }
} 