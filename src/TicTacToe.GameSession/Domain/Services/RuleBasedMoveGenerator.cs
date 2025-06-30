using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameSession.Domain.Services;

/// <summary>
/// Rule-based move generation strategy that uses basic Tic Tac Toe tactics.
/// TODO: Implement win detection, blocking, and strategic positioning.
/// </summary>
public class RuleBasedMoveGenerator : IMoveGenerator
{
    private readonly Random _random = new();

    /// <summary>
    /// Generates a move for the specified player using rule-based strategy.
    /// Currently falls back to random selection as a placeholder.
    /// </summary>
    /// <param name="player">The player to generate a move for.</param>
    /// <param name="board">The current state of the game board.</param>
    /// <returns>A position for the next move.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid moves are available.</exception>
    public Position GenerateMove(Player player, Board board)
    {
        // TODO: Implement rule-based logic:
        // 1. Check if player can win in one move
        // 2. Check if opponent can win in one move and block it
        // 3. Prefer center position if available
        // 4. Prefer corner positions if available
        // 5. Fall back to random selection
        
        var availablePositions = GetAvailablePositions(board);
        
        if (!availablePositions.Any())
        {
            throw new InvalidOperationException("No valid moves available on the board.");
        }
        
        // Placeholder: Currently uses random selection
        // This will be replaced with actual rule-based logic
        var randomIndex = _random.Next(availablePositions.Count);
        return availablePositions[randomIndex];
    }

    /// <summary>
    /// The type of move generation strategy.
    /// </summary>
    public GameStrategy Type => GameStrategy.RuleBased;

    /// <summary>
    /// Gets all available (empty) positions on the board.
    /// </summary>
    /// <param name="board">The game board.</param>
    /// <returns>A list of available positions.</returns>
    private static List<Position> GetAvailablePositions(Board board)
    {
        var availablePositions = new List<Position>();
        
        for (var row = 0; row < 3; row++)
        {
            for (var column = 0; column < 3; column++)
            {
                var position = new Position(row, column);
                if (board.IsPositionEmpty(position))
                {
                    availablePositions.Add(position);
                }
            }
        }
        
        return availablePositions;
    }
} 