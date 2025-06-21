using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Endpoints.DTOs;
using TicTacToe.GameEngine.Persistence;

namespace TicTacToe.GameEngine.Endpoints;

/// <summary>
/// Handles making moves in a Tic Tac Toe game.
/// </summary>
/// <remarks>
/// This endpoint processes a move request for a specific game. It validates the move,
/// updates the game state, checks for win/draw conditions, and persists the changes.
/// The endpoint handles various error conditions including invalid positions, occupied cells,
/// and moves on completed games.
/// </remarks>
public static class MakeMove
{
    /// <summary>
    /// Makes a move in the specified Tic Tac Toe game.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <param name="request">The move request containing the row and column coordinates.</param>
    /// <param name="repository">The game repository for retrieving and persisting the game.</param>
    /// <returns>
    /// An OK (200) response with the updated game state if the move is valid;
    /// a Bad Request (400) response if the move is invalid;
    /// a Not Found (404) response if the game doesn't exist.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        Guid gameId,
        MakeMoveRequest request,
        IGameRepository repository)
    {
        var game = await repository.GetByIdAsync(gameId);
        
        if (game == null)
        {
            return Results.NotFound();
        }

        try
        {
            var position = Position.Create(request.Row, request.Column);
            game.MakeMove(position);
            await repository.SaveAsync(game);

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
        catch (InvalidMoveException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
} 