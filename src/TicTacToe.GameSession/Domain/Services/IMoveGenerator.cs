using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameSession.Domain.Services;

/// <summary>
/// Interface for move generation strategies.
/// </summary>
public interface IMoveGenerator
{
    /// <summary>
    /// Generates a move for the specified player on the given board.
    /// </summary>
    /// <param name="player">The player to generate a move for.</param>
    /// <param name="board">The current state of the game board.</param>
    /// <returns>The position for the next move.</returns>
    Position GenerateMove(Player player, Board board);
    
    /// <summary>
    /// The type of move generation strategy.
    /// </summary>
    GameStrategy Type { get; }
} 