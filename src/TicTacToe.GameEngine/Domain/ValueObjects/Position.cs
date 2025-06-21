namespace TicTacToe.GameEngine.Domain.ValueObjects;

/// <summary>
/// Represents a position on a 3x3 Tic Tac Toe board with row and column coordinates.
/// </summary>
/// <param name="Row">The row index (0-2).</param>
/// <param name="Column">The column index (0-2).</param>
public record Position(int Row, int Column)
{
    /// <summary>
    /// Creates a new Position instance with validation to ensure coordinates are within the 3x3 board bounds.
    /// </summary>
    /// <param name="row">The row index (0-2).</param>
    /// <param name="column">The column index (0-2).</param>
    /// <returns>A new Position instance if the coordinates are valid.</returns>
    /// <exception cref="ArgumentException">Thrown when row or column is outside the valid range (0-2).</exception>
    public static Position Create(int row, int column)
    {
        if (row < 0 || row >= 3 || column < 0 || column >= 3)
        {
            throw new ArgumentException("Position must be within the 3x3 board bounds (0-2).");
        }
        
        return new Position(row, column);
    }
}