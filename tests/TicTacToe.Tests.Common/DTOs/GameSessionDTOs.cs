namespace TicTacToe.Tests.Common.DTOs;

/// <summary>
/// Response from creating a new session.
/// </summary>
public class SessionResponse
{
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Response containing the current state of a session.
/// </summary>
public class SessionStateResponse
{
    public Guid SessionId { get; set; }
    public Guid GameId { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<MoveInfo> Moves { get; set; } = new();
    public string? Winner { get; set; }
}

/// <summary>
/// Response from simulating a game.
/// </summary>
public class SimulationResponse
{
    public Guid SessionId { get; set; }
    public bool IsCompleted { get; set; }
    public string? Winner { get; set; }
    public List<MoveInfo> Moves { get; set; } = new();
}

/// <summary>
/// Information about a move made in the game.
/// </summary>
public class MoveInfo
{
    public int Row { get; set; }
    public int Column { get; set; }
    public string Player { get; set; } = string.Empty;
}

/// <summary>
/// Result of a game simulation for testing purposes.
/// </summary>
public class SimulationResult
{
    public Guid SessionId { get; set; }
    public bool IsCompleted { get; set; }
    public string? Winner { get; set; }
    public List<MoveInfo> Moves { get; set; } = new();
} 