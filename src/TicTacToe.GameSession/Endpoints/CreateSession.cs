using FastEndpoints;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Infrastructure.Persistence;
using TicTacToe.GameSession.Endpoints.DTOs;

namespace TicTacToe.GameSession.Endpoints;

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
        var response = new CreateSessionResponse(session.Id, session.Status.ToString());
        await SendAsync(response, 201, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class CreateSessionEndpoint : CreateSessionEndpointBase
{
    public CreateSessionEndpoint(IGameSessionRepository repository) : base(repository) { }
} 