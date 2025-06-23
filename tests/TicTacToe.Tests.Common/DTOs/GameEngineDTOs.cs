namespace TicTacToe.Tests.Common.DTOs;

/// <summary>
/// Request to create a new game.
/// </summary>
public class CreateGameRequest
{
}

/// <summary>
/// Response from creating a new game.
/// </summary>
public class CreateGameResponse
{
    public Guid GameId { get; set; }
}

/// <summary>
/// Request to make a move in a game.
/// </summary>
public class MakeMoveRequest
{
    public int Row { get; set; }
    public int Column { get; set; }

    public MakeMoveRequest(int row, int column)
    {
        Row = row;
        Column = column;
    }
}

/// <summary>
/// Response containing the current state of a game.
/// </summary>
public class GameStateResponse
{
    public Guid GameId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Winner { get; set; }
    public List<List<string?>> Board { get; set; } = new();
} 