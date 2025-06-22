using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Endpoints.DTOs;
using TicTacToe.GameEngine.Persistence;

namespace TicTacToe.GameEngine.Endpoints;

/// <summary>
/// Handles the creation of new Tic Tac Toe games.
/// </summary>
/// <remarks>
/// This endpoint creates a new game with an empty board and initializes it with Player X as the first player.
/// The game is immediately persisted in the repository and a unique identifier is generated.
/// </remarks>
public static class CreateGame
{
    /// <summary>
    /// Creates a new Tic Tac Toe game and returns the game details.
    /// </summary>
    /// <param name="request">The create game request (currently empty as no parameters are needed).</param>
    /// <param name="repository">The game repository for persisting the new game.</param>
    /// <returns>
    /// A Created (201) response with the new game details including the game ID, status, and current player.
    /// The response includes a Location header pointing to the created game resource.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        CreateGameRequest request,
        IGameRepository repository)
    {
        var game = Game.Create();
        await repository.SaveAsync(game);

        var response = new CreateGameResponse(
            game.Id,
            game.Status,
            game.CurrentPlayer);

        return Results.Created($"/games/{game.Id}", response);
    }
} 