using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;

namespace TicTacToe.GameEngine.Domain.Entities;

public class Board
{
    private readonly Player?[,] _cells;

    public Board()
    {
        _cells = new Player?[3, 3];
    }

    /// <summary>
    /// Creates a Board from a string representation (List&lt;List&lt;string&gt;&gt;).
    /// </summary>
    /// <param name="boardState">The board state as a jagged array of strings.</param>
    /// <returns>A new Board instance with the specified state.</returns>
    public static Board FromStringRepresentation(List<List<string?>> boardState)
    {
        var board = new Board();
        
        for (var row = 0; row < 3 && row < boardState.Count; row++)
        {
            var rowList = boardState[row];
            for (var col = 0; col < 3 && col < rowList.Count; col++)
            {
                var cellValue = rowList[col];
                // Handle both null and empty string as empty cells
                if (!string.IsNullOrEmpty(cellValue) && cellValue != "null")
                {
                    if (Enum.TryParse<Player>(cellValue, out var player))
                    {
                        board.SetCell(new Position(row, col), player);
                    }
                }
            }
        }
        
        return board;
    }

    public bool IsEmpty()
    {
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                if (_cells[row, col].HasValue)
                    return false;
            }
        }
        return true;
    }

    public bool IsCellEmpty(Position position)
    {
        ValidatePosition(position);
        return !_cells[position.Row, position.Column].HasValue;
    }

    /// <summary>
    /// Alias for IsCellEmpty to maintain compatibility with GameSession API.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position is empty; otherwise, false.</returns>
    public bool IsPositionEmpty(Position position)
    {
        return IsCellEmpty(position);
    }

    public Player? GetCell(Position position)
    {
        ValidatePosition(position);
        return _cells[position.Row, position.Column];
    }

    public void SetCell(Position position, Player player)
    {
        ValidatePosition(position);
        _cells[position.Row, position.Column] = player;
    }

    /// <summary>
    /// Alias for SetCell to maintain compatibility with GameSession API.
    /// </summary>
    /// <param name="position">The position to set.</param>
    /// <param name="player">The player to place at the position.</param>
    public void MakeMove(Position position, Player player)
    {
        SetCell(position, player);
    }

    public bool IsFull()
    {
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                if (!_cells[row, col].HasValue)
                    return false;
            }
        }
        return true;
    }

    public Player?[,] GetBoardState()
    {
        return (Player?[,])_cells.Clone();
    }

    /// <summary>
    /// Converts the 3x3 board from a 2D array to a jagged array (List of Lists) for JSON serialization.
    /// </summary>
    /// <remarks>
    /// This method is necessary because System.Text.Json does not support serialization of multi-dimensional arrays.
    /// The board is converted from a 2D array to a jagged array structure that maintains the row/column organization.
    /// </remarks>
    /// <returns>A jagged array (List of Lists) representing the board state, where each element can be null (empty), Player.X, or Player.O.</returns>
    public List<List<Player?>> ToListOfLists()
    {
        var listOfLists = new List<List<Player?>>(3);
        for (var row = 0; row < 3; row++)
        {
            var rowList = new List<Player?>(3);
            for (var col = 0; col < 3; col++)
            {
                rowList.Add(_cells[row, col]);
            }
            listOfLists.Add(rowList);
        }
        return listOfLists;
    }
    
    private void ValidatePosition(Position position)
    {
        if (position.Row < 0 || position.Row >= 3 || position.Column < 0 || position.Column >= 3)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position is outside the board bounds.");
        }
    }
} 