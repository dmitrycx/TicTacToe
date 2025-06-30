namespace TicTacToe.GameSession.Domain.Services;
using TicTacToe.Shared.Enums;

/// <summary>
/// Factory for creating move generators based on the specified move type.
/// Uses dependency injection to receive all registered generators.
/// </summary>
public class MoveGeneratorFactory : IMoveGeneratorFactory
{
    private readonly IReadOnlyDictionary<GameStrategy, IMoveGenerator> _generators;

    /// <summary>
    /// Initializes a new instance of the MoveGeneratorFactory.
    /// </summary>
    /// <param name="generators">Collection of all registered move generators.</param>
    public MoveGeneratorFactory(IEnumerable<IMoveGenerator> generators)
    {
        _generators = generators.ToDictionary(g => g.Type);
    }

    /// <summary>
    /// Creates a move generator for the specified move type.
    /// </summary>
    /// <param name="moveType">The type of move generator to create.</param>
    /// <returns>The move generator instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the move type is not supported.</exception>
    public IMoveGenerator CreateGenerator(GameStrategy moveType)
    {
        if (_generators.TryGetValue(moveType, out var generator))
        {
            return generator;
        }

        throw new ArgumentException($"Move generator for type '{moveType}' is not supported.", nameof(moveType));
    }

    /// <summary>
    /// Gets all supported move types.
    /// </summary>
    /// <returns>Collection of supported move types.</returns>
    public IEnumerable<GameStrategy> GetSupportedMoveTypes()
    {
        return _generators.Keys;
    }
} 