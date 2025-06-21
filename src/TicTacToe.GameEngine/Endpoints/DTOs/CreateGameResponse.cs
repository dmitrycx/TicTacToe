using TicTacToe.GameEngine.Domain.Enums;

namespace TicTacToe.GameEngine.Endpoints.DTOs;

public record CreateGameResponse(Guid GameId, GameStatus Status, Player CurrentPlayer); 