using FastEndpoints;
using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Entities;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameSession.Infrastructure.Persistence;
using TicTacToe.GameSession.Infrastructure.External;
using TicTacToe.GameSession.Infrastructure.External.DTOs;
using TicTacToe.GameSession.Endpoints.DTOs;
using TicTacToe.GameSession.Domain.Constants;
using TicTacToe.GameSession.Domain.Services;

namespace TicTacToe.GameSession.Endpoints;

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
            // Start simulation
            session.StartSimulation();
            await repository.SaveAsync(session);

            // Create game in Game Engine
            var createGameResponse = await gameEngineClient.CreateGameAsync();
            session.SetGameId(createGameResponse.GameId);
            await repository.SaveAsync(session);
            
            // Get the move generator for the specified strategy
            var moveGenerator = moveGeneratorFactory.CreateGenerator(req.MoveStrategy ?? MoveType.Random);
            
            // Simulate moves until game is complete
            var moves = new List<MoveInfo>();
            var currentPlayer = Player.X;
            
            while (session.Status == SessionStatus.InProgress)
            {
                // Get current game state from Game Engine
                var gameState = await gameEngineClient.GetGameStateAsync(session.GameId);
                var board = Board.FromStringRepresentation(gameState.Board);
                
                // Generate move using the selected strategy
                var position = moveGenerator.GenerateMove(currentPlayer, board);
                
                // Make move in Game Engine
                var moveRequest = new MakeMoveRequest(position.Row, position.Column);
                gameState = await gameEngineClient.MakeMoveAsync(session.GameId, moveRequest);
                
                // Record move in session
                session.RecordMove(position, currentPlayer);
                await repository.SaveAsync(session);
                
                moves.Add(new MoveInfo(position.Row, position.Column, currentPlayer.ToString()));

                // Check if game is complete
                if (gameState.Status == SessionConstants.Status.Completed)
                {
                    session.CompleteGame(gameState.Winner);
                    await repository.SaveAsync(session);
                    break;
                }

                // Switch players
                currentPlayer = currentPlayer == Player.X ? Player.O : Player.X;
            }

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
            session.FailSimulation();
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