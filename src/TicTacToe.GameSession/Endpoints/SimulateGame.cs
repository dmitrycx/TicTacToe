using FastEndpoints;
using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Entities;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameSession.Persistence;
using TicTacToe.GameSession.Infrastructure.External;
// DTOs now inlined below
using TicTacToe.GameSession.Domain.Constants;
using TicTacToe.GameSession.Domain.Services;

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
    public MoveType? MoveStrategy { get; set; } = MoveType.Random; // Default to random strategy
}

/// <summary>
/// Endpoint for simulating a complete game.
/// </summary>
public abstract class SimulateGameEndpointBase(
    IGameSessionRepository repository, 
    IGameEngineApiClient gameEngineClient,
    IMoveGeneratorFactory moveGeneratorFactory)
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
        var session = await repository.GetByIdAsync(req.SessionId);
        if (session == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (session.Status != SessionStatus.Created)
        {
            AddError(SessionConstants.ErrorMessages.SessionNotInCreatedState);
            await SendErrorsAsync(400, ct);
            return;
        }

        try
        {
            // Get the move generator for the specified strategy
            var moveGenerator = moveGeneratorFactory.CreateGenerator(req.MoveStrategy ?? MoveType.Random);
            
            // Simulate the game using the domain method
            var moves = await session.SimulateAsync(gameEngineClient, moveGenerator, req.MoveStrategy ?? MoveType.Random);
            
            // Save the updated session
            await repository.SaveAsync(session);

            var response = new SimulateGameResponse(
                session.Id,
                session.Status == SessionStatus.Completed,
                session.Winner?.ToString(),
                moves
            );

            await SendAsync(response, 200, ct);
        }
        catch (Exception)
        {
            // Handle any errors during simulation
            await repository.SaveAsync(session);
            AddError(SessionConstants.ErrorMessages.SimulationFailed);
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
        IMoveGeneratorFactory moveGeneratorFactory) 
        : base(repository, gameEngineClient, moveGeneratorFactory) { }
} 