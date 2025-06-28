using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameSession.Domain.Services;

/// <summary>
/// Random move generation strategy that selects a random valid move from available positions.
/// </summary>
public class RandomMoveGenerator : IMoveGenerator
{
    private readonly Random _random = new();

    /// <summary>
    /// Generates a random move for the specified player on the given board.
    /// </summary>
    /// <param name="player">The player to generate a move for.</param>
    /// <param name="board">The current state of the game board.</param>
    /// <returns>A random valid position for the next move.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid moves are available.</exception>
    public Position GenerateMove(Player player, Board board)
    {
        var availablePositions = GetAvailablePositions(board);
        
        if (!availablePositions.Any())
        {
            throw new InvalidOperationException("No valid moves available on the board.");
        }
        
        // Select a random position from available positions
        var randomIndex = _random.Next(availablePositions.Count);
        var selectedPosition = availablePositions[randomIndex];
        
        return selectedPosition;
    }

    /// <summary>
    /// The type of move generation strategy.
    /// </summary>
    public GameStrategy Type => GameStrategy.Random;

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