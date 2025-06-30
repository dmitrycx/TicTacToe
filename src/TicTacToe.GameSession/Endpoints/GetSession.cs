using FastEndpoints;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Response DTO for getting a session.
/// </summary>
public record GetSessionResponse(
    Guid SessionId, 
    Guid CurrentGameId,
    List<Guid> GameIds,
    string Status, 
    GameStrategy Strategy,
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
/// Endpoint for getting a session by ID.
/// </summary>
public abstract class GetSessionEndpointBase(
    IGameSessionRepository repository,
    ILogger<GetSessionEndpointBase> logger)
    : EndpointWithoutRequest<GetSessionResponse>
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sessionId = Route<Guid>("SessionId");
        logger.LogInformation("Getting session {SessionId}", sessionId);
        
        var session = await repository.GetByIdAsync(sessionId);
        if (session == null)
        {
            logger.LogWarning("Session {SessionId} not found", sessionId);
            await SendNotFoundAsync(ct);
            return;
        }

        logger.LogInformation("Session {SessionId} found. Status: {Status}, CurrentGameId: {CurrentGameId}, GameIdsCount: {GameIdsCount}", 
            sessionId, session.Status, session.CurrentGameId, session.GameIds.Count);

        try
        {
            var response = session.ToResponse();
            logger.LogInformation("Successfully mapped session {SessionId} to response. Response SessionId: {ResponseSessionId}", 
                sessionId, response.SessionId);
            await SendAsync(response, 200, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping session {SessionId} to response", sessionId);
            
            // Try to create a minimal response to see if the issue is with specific fields
            try
            {
                var minimalResponse = new GetSessionResponse(
                    session.Id,
                    session.CurrentGameId,
                    new List<Guid>(),
                    session.Status.ToString(),
                    session.Strategy,
                    session.CreatedAt,
                    null,
                    null,
                    new List<MoveInfo>(),
                    null,
                    null
                );
                logger.LogInformation("Minimal response created successfully");
                await SendAsync(minimalResponse, 200, ct);
            }
            catch (Exception minimalEx)
            {
                logger.LogError(minimalEx, "Error creating minimal response for session {SessionId}", sessionId);
                AddError("Failed to serialize session data");
                await SendErrorsAsync(500, ct);
            }
        }
    }
}

// Concrete implementation for FastEndpoints discovery
public class GetSessionEndpoint : GetSessionEndpointBase
{
    public GetSessionEndpoint(IGameSessionRepository repository, ILogger<GetSessionEndpointBase> logger) 
        : base(repository, logger) { }
} 