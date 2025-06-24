using FastEndpoints;
using TicTacToe.GameSession.Persistence;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Response DTO for getting a session.
/// </summary>
public record GetSessionResponse(
    Guid SessionId, 
    Guid GameId,
    string Status, 
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    List<MoveInfo> Moves, 
    string? Winner,
    string? Result
);

/// <summary>
/// Move information DTO.
/// </summary>
public record MoveInfo(int Row, int Column, string Player);

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

        var response = session.ToResponse();
        await SendAsync(response, 200, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class GetSessionEndpoint : GetSessionEndpointBase
{
    public GetSessionEndpoint(IGameSessionRepository repository) : base(repository) { }
} 