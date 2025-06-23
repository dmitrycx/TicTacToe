namespace TicTacToe.GameSession.Endpoints.DTOs;

/// <summary>
/// Response DTO for creating a session.
/// </summary>
public record CreateSessionResponse(Guid SessionId, string Status);

/// <summary>
/// Response DTO for getting a session.
/// </summary>
public record GetSessionResponse(
    Guid SessionId, 
    Guid GameId,
    string Status, 
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    List<MoveInfo> Moves, 
    string? Winner,
    string? Result
);

/// <summary>
/// Response DTO for game simulation.
/// </summary>
public record SimulateGameResponse(Guid SessionId, bool IsCompleted, string? Winner, List<MoveInfo> Moves);

/// <summary>
/// Move information DTO.
/// </summary>
public record MoveInfo(int Row, int Column, string Player); 