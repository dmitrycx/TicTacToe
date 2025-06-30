using TicTacToe.Shared.Enums;

namespace TicTacToe.Shared.DTOs;

/// <summary>
/// Shared DTO for game state that can be used across services.
/// </summary>
public record GameStateDto(
    Guid GameId,
    GameStatus Status,
    string CurrentPlayer,
    string? Winner,
    List<List<string>> Board,
    DateTime CreatedAt,
    DateTime? LastMoveAt
); 