using FastEndpoints;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Request model for deleting a session.
/// </summary>
public class DeleteSessionRequest
{
    public Guid SessionId { get; set; }
}

/// <summary>
/// Endpoint for deleting a session.
/// </summary>
public abstract class DeleteSessionEndpointBase(IGameSessionRepository repository)
    : Endpoint<DeleteSessionRequest>
{
    public override void Configure()
    {
        Delete("/sessions/{SessionId:guid}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Deletes a session by ID.";
            s.Response(204, "Session successfully deleted.");
            s.Response(404, "Session not found.");
        });
    }

    public override async Task HandleAsync(DeleteSessionRequest req, CancellationToken ct)
    {
        var deleted = await repository.DeleteAsync(req.SessionId);
        
        if (!deleted)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class DeleteSessionEndpoint : DeleteSessionEndpointBase
{
    public DeleteSessionEndpoint(IGameSessionRepository repository) : base(repository) { }
} 