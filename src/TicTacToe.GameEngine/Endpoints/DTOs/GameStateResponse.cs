using TicTacToe.GameEngine.Domain.Enums;

namespace TicTacToe.GameEngine.Endpoints.DTOs;

public record GameStateResponse(
    Guid GameId, 
    GameStatus Status, 
    Player CurrentPlayer, 
    Player? Winner, 
    List<List<Player?>> Board,
    DateTime CreatedAt,
    DateTime? LastMoveAt); 