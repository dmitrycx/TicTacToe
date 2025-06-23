using FastEndpoints;
using TicTacToe.GameSession.Infrastructure.Persistence;
using TicTacToe.GameSession.Endpoints.DTOs;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Request model for getting a session by ID.
/// </summary>
public class GetSessionRequest
{
    public Guid SessionId { get; set; }
}

/// <summary>
/// Endpoint for getting a session by ID.
/// </summary>
public abstract class GetSessionEndpointBase(IGameSessionRepository repository)
    : Endpoint<GetSessionRequest, GetSessionResponse>
{
    public override void Configure()
    {
        Get("/sessions/{SessionId:guid}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Gets a session by ID.";
            s.Response<GetSessionResponse>(200, "The session details.");
            s.Response(404, "Session not found.");
        });
    }

    public override async Task HandleAsync(GetSessionRequest req, CancellationToken ct)
    {
        var session = await repository.GetByIdAsync(req.SessionId);
        if (session == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = new GetSessionResponse(
            session.Id,
            session.GameId,
            session.Status.ToString(),
            session.CreatedAt,
            session.StartedAt,
            session.CompletedAt,
            session.Moves.Select(m => new MoveInfo(m.Position.Row, m.Position.Column, m.Player.ToString())).ToList(),
            session.Winner?.ToString(),
            session.Result?.ToString()
        );

        await SendAsync(response, 200, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class GetSessionEndpoint : GetSessionEndpointBase
{
    public GetSessionEndpoint(IGameSessionRepository repository) : base(repository) { }
} 