using FastEndpoints;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Persistence;

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
public abstract class MakeMoveEndpointBase(IGameRepository repository) : Endpoint<MakeMoveRequest, GameStateResponse>
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
        var game = await repository.GetByIdAsync(req.GameId);
        
        if (game == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        try
        {
            var position = Position.Create(req.Row, req.Column);
            game.MakeMove(position);
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
            AddError(ex.Message);
            await SendErrorsAsync(400, ct);
        }
        catch (ArgumentException ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync(400, ct);
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
    public MakeMoveEndpoint(IGameRepository repository) : base(repository) { }
} 