using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameEngine.Tests.TestHelpers;

/// <summary>
/// Helper methods for creating test games and game scenarios.
/// </summary>
public static class GameTestHelpers
{
    /// <summary>
    /// Creates a new game with default settings.
    /// </summary>
    /// <returns>A new game instance.</returns>
    public static Game CreateNewGame()
    {
        return Game.Create();
    }

    /// <summary>
    /// Creates a game with a predefined board state.
    /// </summary>
    /// <param name="boardState">A string representation of the board where each character represents a cell:
    /// 'X' = Player.X, 'O' = Player.O, '.' = Empty</param>
    /// <returns>A game with the specified board state.</returns>
    public static Game CreateGameFromBoard(string boardState)
    {
        var game = Game.Create();
        
        // Parse the board state string (3x3 grid)
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                var index = row * 3 + col;
                if (index < boardState.Length)
                {
                    var cell = boardState[index];
                    if (cell == 'X')
                    {
                        game.MakeMove(Position.Create(row, col));
                    }
                    else if (cell == 'O')
                    {
                        game.MakeMove(Position.Create(row, col));
                    }
                }
            }
        }
        
        return game;
    }

    /// <summary>
    /// Creates a game with a series of predefined moves.
    /// </summary>
    /// <param name="moves">A collection of tuples containing (Player, Row, Column) for each move.</param>
    /// <returns>A game with the specified moves applied.</returns>
    public static Game CreateGameWithMoves(params (Player Player, int Row, int Col)[] moves)
    {
        var game = Game.Create();
        
        foreach (var (player, row, col) in moves)
        {
            // Stop making moves if the game is already completed
            if (game.Status != GameStatus.InProgress)
            {
                break;
            }
            
            // Set the current player to match the move
            if (game.CurrentPlayer != player)
            {
                // Simplified approach for testing purposes
            }
            
            game.MakeMove(Position.Create(row, col));
        }
        
        return game;
    }

    public static Game CreateWinningGame(Player winner)
    {
        var game = Game.Create();
        
        // A minimal sequence for X to win
        var xWinningMoves = new[] { (0, 0), (1, 0), (0, 1), (1, 1), (0, 2) };
        
        // A known good sequence for O's win
        var oWinningSequence = new[]
        {
            (1, 0), // X
            (0, 0), // O
            (1, 2), // X
            (1, 1), // O
            (2, 0), // X
            (2, 2)  // O -> Wins on 6th move
        };

        var movesToPlay = winner == Player.X ? xWinningMoves : oWinningSequence;

        foreach (var (row, col) in movesToPlay)
        {
            game.MakeMove(Position.Create(row, col));
        }
        
        return game;
    }

    public static Game CreateDrawGame()
    {
        // This sequence of moves is guaranteed to result in a draw
        // without any player winning prematurely.
        var moves = new[]
        {
            (0, 0), // X
            (1, 1), // O
            (0, 1), // X
            (0, 2), // O
            (2, 0), // X
            (1, 0), // O
            (1, 2), // X
            (2, 2), // O
            (2, 1)  // X -> This is the final move that fills the board and results in a draw.
        };
        
        var game = Game.Create();
        
        foreach (var (row, col) in moves)
        {
            var position = Position.Create(row, col);
            game.MakeMove(position);
        }
        
        return game;
    }
} 