using FastEndpoints;
using TicTacToe.GameSession.Infrastructure.Persistence;
using TicTacToe.GameSession.Endpoints.DTOs;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Response DTO for listing sessions.
/// </summary>
public record ListSessionsResponse(List<SessionSummary> Sessions);

/// <summary>
/// Summary information for a session.
/// </summary>
public record SessionSummary(Guid SessionId, string Status, DateTime CreatedAt, int MoveCount, string? Winner);

/// <summary>
/// Endpoint for listing all sessions.
/// </summary>
public abstract class ListSessionsEndpointBase(IGameSessionRepository repository) : EndpointWithoutRequest<ListSessionsResponse>
{
    public override void Configure()
    {
        Get("/sessions");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Lists all game sessions.";
            s.Response<ListSessionsResponse>(200, "List of all sessions.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sessions = await repository.GetAllAsync();
        
        var sessionSummaries = sessions.Select(s => new SessionSummary(
            s.Id,
            s.Status.ToString(),
            s.CreatedAt,
            s.Moves.Count,
            s.Winner
        )).ToList();

        var response = new ListSessionsResponse(sessionSummaries);
        await SendAsync(response, 200, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class ListSessionsEndpoint : ListSessionsEndpointBase
{
    public ListSessionsEndpoint(IGameSessionRepository repository) : base(repository) { }
} 