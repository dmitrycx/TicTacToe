using FastEndpoints;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Response DTO for creating a session.
/// </summary>
public record CreateSessionResponse(Guid SessionId, Guid CurrentGameId, List<Guid> GameIds, string Status);

/// <summary>
/// Endpoint for creating a new game session.
/// </summary>
public abstract class CreateSessionEndpointBase(IGameSessionRepository repository) : EndpointWithoutRequest<CreateSessionResponse>
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        var session = Domain.Aggregates.GameSession.Create();
        await repository.SaveAsync(session);
        var response = new CreateSessionResponse(session.Id, session.CurrentGameId, session.GameIds.ToList(), session.Status.ToString());
        await SendAsync(response, 201, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class CreateSessionEndpoint : CreateSessionEndpointBase
{
    public CreateSessionEndpoint(IGameSessionRepository repository) : base(repository) { }
} 