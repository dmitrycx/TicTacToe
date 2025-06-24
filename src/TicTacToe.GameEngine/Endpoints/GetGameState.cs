using FastEndpoints;
using TicTacToe.GameEngine.Persistence;

namespace TicTacToe.GameEngine.Endpoints;

/// <summary>
/// Request model for getting game state.
/// </summary>
public class GetGameStateRequest
{
    public Guid GameId { get; set; }
}

/// <summary>
/// Response model for game state.
/// </summary>
public record GameStateResponse(
    Guid GameId,
    string Status,
    string CurrentPlayer,
    string? Winner,
    List<List<string>> Board,
    DateTime CreatedAt,
    DateTime? LastMoveAt);

/// <summary>
/// Endpoint for retrieving the current state of a Tic Tac Toe game.
/// </summary>
public abstract class GetGameStateEndpointBase(IGameRepository repository) : Endpoint<GetGameStateRequest, GameStateResponse>
{
    public override void Configure()
    {
        Get("/games/{GameId:guid}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Retrieves the current state of a Tic Tac Toe game.";
            s.Description = "Returns the complete game state including the board, current player, game status, and winner (if any).";
            s.Response<GameStateResponse>(200, "The current game state.");
            s.Response(404, "Game not found.");
        });
    }

    public override async Task HandleAsync(GetGameStateRequest req, CancellationToken ct)
    {
        var game = await repository.GetByIdAsync(req.GameId);
        
        if (game == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

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

    private static List<List<string>> ConvertBoardToStrings(List<List<Domain.Enums.Player?>> board)
    {
        return board.Select(row => 
            row.Select(cell => cell?.ToString() ?? "").ToList()
        ).ToList();
    }
}

// Concrete implementation for FastEndpoints discovery
public class GetGameStateEndpoint : GetGameStateEndpointBase
{
    public GetGameStateEndpoint(IGameRepository repository) : base(repository) { }
} 