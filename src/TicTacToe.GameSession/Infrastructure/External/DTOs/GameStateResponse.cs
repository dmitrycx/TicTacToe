namespace TicTacToe.GameSession.Infrastructure.External.DTOs;

/// <summary>
/// Response containing the current game state from the Game Engine Service.
/// </summary>
public record GameStateResponse(
    Guid GameId,
    string Status,
    string CurrentPlayer,
    string? Winner,
    List<List<string?>> Board,
    DateTime CreatedAt,
    DateTime? LastMoveAt); 