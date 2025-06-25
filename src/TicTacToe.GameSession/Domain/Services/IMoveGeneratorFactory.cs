namespace TicTacToe.GameSession.Domain.Services;

/// <summary>
/// Factory interface for creating move generators.
/// </summary>
public interface IMoveGeneratorFactory
{
    /// <summary>
    /// Creates a move generator for the specified strategy type.
    /// </summary>
    /// <param name="moveType">The type of move generation strategy.</param>
    /// <returns>A move generator instance.</returns>
    IMoveGenerator CreateGenerator(MoveType moveType);

    /// <summary>
    /// Gets all supported move generation strategies.
    /// </summary>
    /// <returns>A collection of supported move types.</returns>
    IEnumerable<MoveType> GetSupportedMoveTypes();
} 