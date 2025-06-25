using FastEndpoints;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Persistence;

namespace TicTacToe.GameEngine.Endpoints;

/// <summary>
/// Response model for creating a game.
/// </summary>
public record CreateGameResponse(Guid GameId, string Status, string CurrentPlayer);

/// <summary>
/// Endpoint for creating new Tic Tac Toe games.
/// </summary>
public abstract class CreateGameEndpointBase(IGameRepository repository) : EndpointWithoutRequest<CreateGameResponse>
{
    public override void Configure()
    {
        Post("/games");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Creates a new Tic Tac Toe game.";
            s.Description = "Creates a new game with an empty board and initializes it with Player X as the first player. The game is immediately persisted in the repository and a unique identifier is generated.";
            s.Response<CreateGameResponse>(201, "The created game details.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var game = Game.Create();
        await repository.SaveAsync(game);

        var response = new CreateGameResponse(
            game.Id,
            game.Status.ToString(),
            game.CurrentPlayer.ToString());

        await SendAsync(response, 201, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class CreateGameEndpoint : CreateGameEndpointBase
{
    public CreateGameEndpoint(IGameRepository repository) : base(repository) { }
} 