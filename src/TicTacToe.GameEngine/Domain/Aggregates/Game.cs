using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameEngine.Domain.Aggregates;

/// <summary>
/// Represents a Tic Tac Toe game aggregate.
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

    /// <summary>
    /// Initializes a new instance of the Game class.
    /// </summary>
    public Game()
    {
        Id = Guid.NewGuid();
        Board = new Board();
        CurrentPlayer = Player.X;
        Status = GameStatus.InProgress;
        CreatedAt = DateTime.UtcNow;
    }

    public static Game Create()
    {
        return new Game();
    }

    /// <summary>
    /// Makes a move on the board.
    /// </summary>
    /// <param name="position">The position to place the move.</param>
    /// <param name="player">The player making the move.</param>
    /// <exception cref="InvalidMoveException">Thrown when the move is invalid.</exception>
    public void MakeMove(Position position, Player player)
    {
        ValidateMove(position, player);
        Board.SetCell(position, player);
        LastMoveAt = DateTime.UtcNow;
        CheckGameStatus(player);
        if (Status == GameStatus.InProgress)
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }
    }

    /// <summary>
    /// Makes a move on the board using the current player.
    /// </summary>
    /// <param name="position">The position to place the move.</param>
    /// <exception cref="InvalidMoveException">Thrown when the move is invalid.</exception>
    public void MakeMove(Position position)
    {
        MakeMove(position, CurrentPlayer);
    }

    private void ValidateMove(Position position, Player player)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidMoveException("Cannot make a move on a completed game.");
        }

        if (player != CurrentPlayer)
        {
            throw new InvalidMoveException($"It's not {player}'s turn. Current player is {CurrentPlayer}.");
        }

        if (!Board.IsCellEmpty(position))
        {
            throw new InvalidMoveException($"Position {position} is already occupied.");
        }
    }

    private void CheckGameStatus(Player movePlayer)
    {
        if (HasWinningLine(movePlayer))
        {
            Status = GameStatus.Win;
            Winner = movePlayer;
        }
        else if (Board.IsFull())
        {
            Status = GameStatus.Draw;
        }
    }

    private bool HasWinningLine(Player player)
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (IsWinningLine(row, 0, 0, 1, player)) return true;
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (IsWinningLine(0, col, 1, 0, player)) return true;
        }

        // Check diagonals
        if (IsWinningLine(0, 0, 1, 1, player)) return true;
        if (IsWinningLine(0, 2, 1, -1, player)) return true;

        return false;
    }

    private bool IsWinningLine(int startRow, int startCol, int deltaRow, int deltaCol, Player player)
    {
        var firstCell = Board.GetCell(Position.Create(startRow, startCol));
        if (firstCell != player) return false;

        for (int i = 1; i < 3; i++)
        {
            var currentCell = Board.GetCell(Position.Create(startRow + i * deltaRow, startCol + i * deltaCol));
            if (currentCell != player) return false;
        }

        return true;
    }

    /// <summary>
    /// Sets the game properties from database record (for repository mapping).
    /// </summary>
    /// <param name="id">The game ID.</param>
    /// <param name="board">The game board.</param>
    /// <param name="currentPlayer">The current player.</param>
    /// <param name="status">The game status.</param>
    /// <param name="winner">The winner.</param>
    /// <param name="createdAt">When the game was created.</param>
    /// <param name="lastMoveAt">When the last move was made.</param>
    public void SetProperties(
        Guid id,
        Board board,
        Player currentPlayer,
        GameStatus status,
        Player? winner,
        DateTime createdAt,
        DateTime? lastMoveAt)
    {
        Id = id;
        Board = board;
        CurrentPlayer = currentPlayer;
        Status = status;
        Winner = winner;
        CreatedAt = createdAt;
        LastMoveAt = lastMoveAt;
    }
} 