using FastEndpoints;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Request DTO for creating a session.
/// </summary>
public record CreateSessionRequest(GameStrategy Strategy = GameStrategy.Random);

/// <summary>
/// Response DTO for creating a session.
/// </summary>
public record CreateSessionResponse(Guid SessionId, Guid CurrentGameId, List<Guid> GameIds, string Status, GameStrategy Strategy);

/// <summary>
/// Endpoint for creating a new game session.
/// </summary>
public abstract class CreateSessionEndpointBase(IGameSessionRepository repository) : Endpoint<CreateSessionRequest, CreateSessionResponse>
{
    public override void Configure()
    {
        Post("/sessions");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Creates a new game session.";
            s.Response<CreateSessionResponse>(201, "The created session details.");
        });
    }

    public override async Task HandleAsync(CreateSessionRequest req, CancellationToken ct)
    {
        var session = Domain.Aggregates.GameSession.Create(req.Strategy);
        await repository.SaveAsync(session);
        var response = new CreateSessionResponse(session.Id, session.CurrentGameId, session.GameIds.ToList(), session.Status.ToString(), session.Strategy);
        await SendAsync(response, 201, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class CreateSessionEndpoint : CreateSessionEndpointBase
{
    public CreateSessionEndpoint(IGameSessionRepository repository) : base(repository) { }
} 