using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;

namespace TicTacToe.GameEngine.Domain.Aggregates;

/// <summary>
/// Represents a Tic Tac Toe game aggregate root that manages the game state,
/// validates moves, and determines game outcomes.
/// </summary>
public class Game
{
    public Guid Id { get; private set; }
    public Board Board { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public GameStatus Status { get; private set; }
    public Player? Winner { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastMoveAt { get; private set; }

    private Game()
    {
        Board = new Board();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new Tic Tac Toe game with an empty board and initial state.
    /// </summary>
    /// <returns>A new Game instance with Player X as the first player and InProgress status.</returns>
    public static Game Create()
    {
        return new Game
        {
            Id = Guid.NewGuid(),
            CurrentPlayer = Player.X,
            Status = GameStatus.InProgress
        };
    }

    /// <summary>
    /// Makes a move on the game board at the specified position.
    /// </summary>
    /// <param name="position">The position on the board where the move should be made.</param>
    /// <exception cref="InvalidMoveException">Thrown when the move is invalid (e.g., position out of bounds, cell occupied, or game already ended).</exception>
    public void MakeMove(Position position)
    {
        ValidateMove(position);

        var movePlayer = CurrentPlayer;
        Board.SetCell(position, movePlayer);
        LastMoveAt = DateTime.UtcNow;

        CheckGameStatus(movePlayer);

        if (Status == GameStatus.InProgress)
        {
            SwitchPlayer();
        }
    }

    private void ValidateMove(Position position)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidMoveException("Cannot make a move on a completed game.");
        }

        if (!Board.IsCellEmpty(position))
        {
            throw new InvalidMoveException($"Cell at position ({position.Row}, {position.Column}) is already occupied.");
        }
    }

    private void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
    }

    private void CheckGameStatus(Player movePlayer)
    {
        var hasWinner = HasWinner();
        if (hasWinner)
        {
            Status = GameStatus.Win;
            Winner = movePlayer;
        }
        else if (Board.IsFull())
        {
            Status = GameStatus.Draw;
            Winner = null;
        }
    }

    private bool HasWinner()
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (CheckLine(row, 0, 0, 1)) return true;
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (CheckLine(0, col, 1, 0)) return true;
        }

        // Check diagonals
        if (CheckLine(0, 0, 1, 1)) return true;
        if (CheckLine(0, 2, 1, -1)) return true;

        return false;
    }

    private bool CheckLine(int startRow, int startCol, int deltaRow, int deltaCol)
    {
        var firstCell = Board.GetCell(Position.Create(startRow, startCol));
        if (!firstCell.HasValue) return false;

        for (int i = 1; i < 3; i++)
        {
            var currentCell = Board.GetCell(Position.Create(startRow + i * deltaRow, startCol + i * deltaCol));
            if (currentCell != firstCell) return false;
        }

        return true;
    }
} 