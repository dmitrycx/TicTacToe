using FastEndpoints;

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
        
        var response = sessions.ToResponse();
        await SendAsync(response, 200, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class ListSessionsEndpoint : ListSessionsEndpointBase
{
    public ListSessionsEndpoint(IGameSessionRepository repository) : base(repository) { }
} 