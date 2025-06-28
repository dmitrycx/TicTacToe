using FastEndpoints;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Persistence;
using Microsoft.Extensions.Logging;

namespace TicTacToe.GameEngine.Endpoints;

/// <summary>
/// Request model for making a move.
/// </summary>
public class MakeMoveRequest
{
    public Guid GameId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}

/// <summary>
/// Endpoint for making moves in a Tic Tac Toe game.
/// </summary>
public abstract class MakeMoveEndpointBase(IGameRepository repository, ILogger<MakeMoveEndpointBase> logger) : Endpoint<MakeMoveRequest, GameStateResponse>
{
    public override void Configure()
    {
        Post("/games/{GameId:guid}/move");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Makes a move in a Tic Tac Toe game.";
            s.Description = "Processes a move request for a specific game. It validates the move, updates the game state, checks for win/draw conditions, and persists the changes.";
            s.Response<GameStateResponse>(200, "The updated game state.");
            s.Response(400, "Invalid move (position occupied, out of bounds, or game completed).");
            s.Response(404, "Game not found.");
        });
    }

    public override async Task HandleAsync(MakeMoveRequest req, CancellationToken ct)
    {
        logger.LogInformation("Processing move request for game {GameId}: Row={Row}, Column={Column}", req.GameId, req.Row, req.Column);
        
        var game = await repository.GetByIdAsync(req.GameId);
        if (game == null)
        {
            logger.LogWarning("Game {GameId} not found", req.GameId);
            await SendNotFoundAsync(ct);
            return;
        }

        logger.LogInformation("Game {GameId} found. Current status: {Status}, Current player: {CurrentPlayer}", 
            req.GameId, game.Status, game.CurrentPlayer);

        try
        {
            // Log the current board state before the move
            var currentBoard = game.Board.ToListOfLists();
            logger.LogInformation("Current board state for game {GameId}: {BoardState}", req.GameId, 
                string.Join("|", currentBoard.Select(row => string.Join(",", row.Select(cell => cell?.ToString() ?? "null")))));

            var position = Position.Create(req.Row, req.Column);
            logger.LogInformation("Created position for game {GameId}: Row={Row}, Column={Column}", req.GameId, position.Row, position.Column);
            
            // Check if the position is valid before making the move
            if (!game.Board.IsCellEmpty(position))
            {
                logger.LogWarning("Invalid move for game {GameId}: Cell at ({Row}, {Column}) is already occupied", req.GameId, req.Row, req.Column);
                AddError($"Cell at position ({req.Row}, {req.Column}) is already occupied.");
                await SendErrorsAsync(400, ct);
                return;
            }

            game.MakeMove(position);
            logger.LogInformation("Move successful for game {GameId}. New status: {Status}, Winner: {Winner}", 
                req.GameId, game.Status, game.Winner);
            
            await repository.SaveAsync(game);

            var response = new GameStateResponse(
                game.Id,
                game.Status.ToString(),
                game.CurrentPlayer.ToString(),
                game.Winner?.ToString(),
                ConvertBoardToStrings(game.Board.ToListOfLists()),
                game.CreatedAt,
                game.LastMoveAt);

            await SendAsync(response, 200, ct);
        }
        catch (InvalidMoveException ex)
        {
            logger.LogError(ex, "Invalid move exception for game {GameId}: {Message}", req.GameId, ex.Message);
            AddError(ex.Message);
            await SendErrorsAsync(400, ct);
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Argument exception for game {GameId}: {Message}", req.GameId, ex.Message);
            AddError(ex.Message);
            await SendErrorsAsync(400, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error making move for game {GameId}: {Message}", req.GameId, ex.Message);
            AddError($"Unexpected error: {ex.Message}");
            await SendErrorsAsync(500, ct);
        }
    }

    private static List<List<string>> ConvertBoardToStrings(List<List<Domain.Enums.Player?>> board)
    {
        return board.Select(row => 
            row.Select(cell => cell?.ToString() ?? "").ToList()
        ).ToList();
    }
}

// Concrete implementation for FastEndpoints discovery
public class MakeMoveEndpoint : MakeMoveEndpointBase
{
    public MakeMoveEndpoint(IGameRepository repository, ILogger<MakeMoveEndpointBase> logger) : base(repository, logger) { }
} 