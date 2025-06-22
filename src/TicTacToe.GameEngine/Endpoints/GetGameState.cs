using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Endpoints.DTOs;
using TicTacToe.GameEngine.Persistence;

namespace TicTacToe.GameEngine.Endpoints;

/// <summary>
/// Handles retrieval of the current state of a Tic Tac Toe game.
/// </summary>
/// <remarks>
/// This endpoint retrieves the complete game state including the board layout, current player,
/// game status, winner (if any), and timestamps. The board is returned as a flat list for
/// easy serialization and consumption by clients.
/// </remarks>
public static class GetGameState
{
    /// <summary>
    /// Retrieves the current state of a Tic Tac Toe game by its unique identifier.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game to retrieve.</param>
    /// <param name="repository">The game repository for retrieving the game.</param>
    /// <returns>
    /// An OK (200) response with the complete game state if the game exists;
    /// otherwise, a Not Found (404) response.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        Guid gameId,
        IGameRepository repository)
    {
        var game = await repository.GetByIdAsync(gameId);
        
        if (game == null)
        {
            return Results.NotFound();
        }

        var response = new GameStateResponse(
            game.Id,
            game.Status,
            game.CurrentPlayer,
            game.Winner,
            game.Board.ToListOfLists(),
            game.CreatedAt,
            game.LastMoveAt);

        return Results.Ok(response);
    }
} 