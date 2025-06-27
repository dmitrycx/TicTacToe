using FastEndpoints;
// DTOs now inlined below
using TicTacToe.GameSession.Domain.Constants;
using Microsoft.Extensions.Logging;
using TicTacToe.GameSession.Services;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Response from creating a new game in the Game Engine Service.
/// </summary>
public record CreateGameResponse(Guid GameId, DateTime CreatedAt);

/// <summary>
/// Response containing the current game state from the Game Engine Service.
/// </summary>
public record GameStateResponse(
    Guid GameId,
    string Status,
    string CurrentPlayer,
    string? Winner,
    List<List<string?>> Board,
    DateTime CreatedAt,
    DateTime? LastMoveAt);

/// <summary>
/// Request for making a move in the Game Engine Service.
/// </summary>
public record MakeMoveRequest(int Row, int Column);

/// <summary>
/// Response DTO for game simulation.
/// </summary>
public record SimulateGameResponse(Guid SessionId, bool IsCompleted, string? Winner, List<MoveInfo> Moves);

/// <summary>
/// Request model for simulating a game.
/// </summary>
public class SimulateGameRequest
{
    public Guid SessionId { get; set; }
    public GameStrategy? MoveStrategy { get; set; } = GameStrategy.Random; // Default to random strategy
}

/// <summary>
/// Endpoint for simulating a complete game.
/// </summary>
public abstract class SimulateGameEndpointBase(
    IGameSessionRepository repository, 
    IGameEngineApiClient gameEngineClient,
    IMoveGeneratorFactory moveGeneratorFactory,
    ILogger<SimulateGameEndpointBase> logger,
    ISignalRNotificationService signalRNotificationService)
    : Endpoint<SimulateGameRequest, SimulateGameResponse>
{
    public override void Configure()
    {
        Post("/sessions/{SessionId:guid}/simulate");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Simulates a complete game for the specified session.";
            s.Response<SimulateGameResponse>(200, "The simulation results.");
            s.Response(404, "Session not found.");
            s.Response(400, SessionConstants.ErrorMessages.SessionNotInCreatedState);
        });
    }

    public override async Task HandleAsync(SimulateGameRequest req, CancellationToken ct)
    {
        logger.LogInformation("Starting simulation for session {SessionId}", req.SessionId);
        
        var session = await repository.GetByIdAsync(req.SessionId);
        if (session == null)
        {
            logger.LogWarning("Session {SessionId} not found", req.SessionId);
            await SendNotFoundAsync(ct);
            return;
        }

        if (session.Status != SessionStatus.Created)
        {
            logger.LogWarning("Session {SessionId} is not in Created state. Current state: {Status}", req.SessionId, session.Status);
            AddError(SessionConstants.ErrorMessages.SessionNotInCreatedState);
            await SendErrorsAsync(400, ct);
            return;
        }

        try
        {
            logger.LogInformation("Creating move generator for strategy {Strategy}", req.MoveStrategy ?? GameStrategy.Random);
            // Get the move generator for the specified strategy
            var moveGenerator = moveGeneratorFactory.CreateGenerator(req.MoveStrategy ?? GameStrategy.Random);
            
            logger.LogInformation("Starting game simulation for session {SessionId}", req.SessionId);
            // Simulate the game using the domain method
            var moves = await session.SimulateAsync(gameEngineClient, moveGenerator, req.MoveStrategy ?? GameStrategy.Random);
            
            logger.LogInformation("Simulation completed successfully for session {SessionId}. Total moves: {MoveCount}", req.SessionId, moves.Count);
            
            // Save the updated session
            await repository.SaveAsync(session);

            var response = new SimulateGameResponse(
                session.Id,
                session.Status == SessionStatus.Completed,
                session.Winner?.ToString(),
                moves
            );

            // Send SignalR notification to UI
            try
            {
                // Convert the moves to a board array that the UI expects
                var board = new string?[9];
                foreach (var move in moves)
                {
                    var index = move.Row * 3 + move.Column;
                    board[index] = move.Player;
                }
                
                var finalGameState = new
                {
                    board = board,
                    status = session.Status == SessionStatus.Completed ? "win" : "in_progress",
                    winner = session.Winner,
                    currentPlayer = session.Status == SessionStatus.Completed ? null : "X"
                };
                
                await signalRNotificationService.NotifyGameCompletedAsync(session.Id.ToString(), finalGameState);
                logger.LogInformation("Sent SignalR notification for completed simulation to session {SessionId}", req.SessionId);
            }
            catch (Exception notificationEx)
            {
                logger.LogWarning(notificationEx, "Failed to send SignalR notification for session {SessionId}", req.SessionId);
                // Don't fail the request if SignalR notification fails
            }

            await SendAsync(response, 200, ct);
        }
        catch (HttpRequestException httpEx)
        {
            logger.LogError(httpEx, "HTTP communication error during simulation for session {SessionId}: {Message}", req.SessionId, httpEx.Message);
            await repository.SaveAsync(session);
            AddError($"HTTP communication failed: {httpEx.Message}");
            await SendErrorsAsync(500, ct);
        }
        catch (InvalidOperationException invalidOpEx)
        {
            logger.LogError(invalidOpEx, "Invalid operation during simulation for session {SessionId}: {Message}", req.SessionId, invalidOpEx.Message);
            await repository.SaveAsync(session);
            AddError($"Invalid operation: {invalidOpEx.Message}");
            await SendErrorsAsync(500, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during simulation for session {SessionId}: {Message}", req.SessionId, ex.Message);
            // Handle any errors during simulation
            await repository.SaveAsync(session);
            AddError($"Simulation failed: {ex.Message}");
            await SendErrorsAsync(500, ct);
        }
    }
}

// Concrete implementation for FastEndpoints discovery
public class SimulateGameEndpoint : SimulateGameEndpointBase
{
    public SimulateGameEndpoint(
        IGameSessionRepository repository, 
        IGameEngineApiClient gameEngineClient,
        IMoveGeneratorFactory moveGeneratorFactory,
        ILogger<SimulateGameEndpointBase> logger,
        ISignalRNotificationService signalRNotificationService) 
        : base(repository, gameEngineClient, moveGeneratorFactory, logger, signalRNotificationService) { }
} 