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
/// Response DTO for deleting a session.
/// </summary>
public record DeleteSessionResponse(bool Success, string Message);

/// <summary>
/// Endpoint for deleting a session.
/// </summary>
public abstract class DeleteSessionEndpointBase(IGameSessionRepository repository)
    : Endpoint<DeleteSessionRequest, DeleteSessionResponse>
{
    public override void Configure()
    {
        Delete("/sessions/{SessionId:guid}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Deletes a session by ID.";
            s.Response<DeleteSessionResponse>(200, "Session successfully deleted.");
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

        var response = new DeleteSessionResponse(true, $"Session {req.SessionId} successfully deleted.");
        await SendAsync(response, 200, ct);
    }
}

// Concrete implementation for FastEndpoints discovery
public class DeleteSessionEndpoint : DeleteSessionEndpointBase
{
    public DeleteSessionEndpoint(IGameSessionRepository repository) : base(repository) { }
} 