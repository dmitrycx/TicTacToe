namespace TicTacToe.GameSession.Infrastructure.External.DTOs;

/// <summary>
/// Request for making a move in the Game Engine Service.
/// </summary>
public record MakeMoveRequest(
    int Row,
    int Column); 