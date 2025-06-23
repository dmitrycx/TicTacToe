namespace TicTacToe.GameSession.Infrastructure.External.DTOs;

/// <summary>
/// Response from creating a new game in the Game Engine Service.
/// </summary>
public record CreateGameResponse(
    Guid GameId,
    DateTime CreatedAt); 