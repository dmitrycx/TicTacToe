using FastEndpoints;
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

namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Request model for simulating a game.
/// </summary>
public class SimulateGameRequest
{
    public Guid SessionId { get; set; }
}

/// <summary>
/// Endpoint for simulating a complete game.
/// </summary>
public abstract class SimulateGameEndpointBase(IGameSessionRepository repository, IGameEngineApiClient gameEngineClient)
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
            session.SetGameId(createGameResponse.GameId); // Set the game ID from the response
            await repository.SaveAsync(session);
            
            // Simulate moves until game is complete
            var moves = new List<MoveInfo>();
            var currentPlayer = Player.X;
            
            while (session.Status == SessionStatus.InProgress)
            {
                // Generate a random move
                var move = GenerateRandomMove(session);
                if (move == null)
                    break; // No more valid moves

                // Make move in Game Engine
                var moveRequest = new MakeMoveRequest(move.Position.Row, move.Position.Column);
                var gameState = await gameEngineClient.MakeMoveAsync(session.GameId, moveRequest);
                
                // Record move in session
                session.RecordMove(move.Position, move.Player);
                await repository.SaveAsync(session);
                
                moves.Add(new MoveInfo(move.Position.Row, move.Position.Column, move.Player.ToString()));

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

    private static Move? GenerateRandomMove(Domain.Aggregates.GameSession session)
    {
        var random = new Random();
        var attempts = 0;
        const int maxAttempts = 100;

        while (attempts < maxAttempts)
        {
            var row = random.Next(0, 3);
            var column = random.Next(0, 3);
            var position = new Position(row, column);

            // Check if position is available
            if (!session.Moves.Any(m => m.Position.Equals(position)))
            {
                var player = session.Moves.Count % 2 == 0 ? Player.X : Player.O;
                return new Move(session.Id, player, position, MoveType.Random, session.Moves.Count + 1);
            }

            attempts++;
        }

        return null; // No valid moves found
    }
}

// Concrete implementation for FastEndpoints discovery
public class SimulateGameEndpoint : SimulateGameEndpointBase
{
    public SimulateGameEndpoint(IGameSessionRepository repository, IGameEngineApiClient gameEngineClient) : base(repository, gameEngineClient) { }
} 