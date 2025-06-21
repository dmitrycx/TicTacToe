using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Domain.Exceptions;

namespace TicTacToe.GameEngine.Domain.Entities;

public class Board
{
    private readonly Player?[,] _cells;

    public Board()
    {
        _cells = new Player?[3, 3];
    }

    public bool IsEmpty()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
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

    public bool IsFull()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
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
        for (int row = 0; row < 3; row++)
        {
            var rowList = new List<Player?>(3);
            for (int col = 0; col < 3; col++)
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