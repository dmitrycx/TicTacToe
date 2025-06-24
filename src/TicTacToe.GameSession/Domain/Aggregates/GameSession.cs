using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameSession.Domain.Constants;
using TicTacToe.GameSession.Domain.Services;
using TicTacToe.GameSession.Infrastructure.External;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Domain.Aggregates;

/// <summary>
/// Represents a game session that manages the lifecycle of a Tic Tac Toe game.
/// This aggregate root coordinates between the Game Engine Service and manages session state.
/// </summary>
public class GameSession
{
    private readonly List<Move> _moves = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Unique identifier for the session.
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Identifier of the game in the Game Engine Service.
    /// </summary>
    public Guid GameId { get; private set; }
    
    /// <summary>
    /// Current status of the session.
    /// </summary>
    public SessionStatus Status { get; private set; }
    
    /// <summary>
    /// When the session was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// When the game simulation started.
    /// </summary>
    public DateTime? StartedAt { get; private set; }
    
    /// <summary>
    /// When the game was completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }
    
    /// <summary>
    /// All moves made in this session.
    /// </summary>
    public IReadOnlyCollection<Move> Moves => _moves.AsReadOnly();
    
    /// <summary>
    /// Result of the game (if completed).
    /// </summary>
    public GameStatus? Result { get; private set; }
    
    /// <summary>
    /// Winner of the game (if completed).
    /// </summary>
    public string? Winner { get; private set; }
    
    /// <summary>
    /// Domain events that have occurred in this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Creates a new game session.
    /// </summary>
    public static GameSession Create()
    {
        var session = new GameSession();
        session.Id = Guid.NewGuid();
        session.Status = SessionStatus.Created;
        session.CreatedAt = DateTime.UtcNow;
        
        session._domainEvents.Add(new SessionCreatedEvent(session.Id, Guid.Empty));
        return session;
    }

    /// <summary>
    /// Creates a new game session with a specific game ID.
    /// </summary>
    /// <param name="gameId">The game ID from the Game Engine Service.</param>
    public GameSession(Guid gameId)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        Status = SessionStatus.Created;
        CreatedAt = DateTime.UtcNow;
        
        _domainEvents.Add(new SessionCreatedEvent(Id, GameId));
    }

    /// <summary>
    /// Sets the game ID from the Game Engine Service.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    public void SetGameId(Guid gameId)
    {
        if (GameId != Guid.Empty)
        {
            throw new InvalidSessionStateException(SessionConstants.ErrorMessages.GameIdAlreadySet);
        }
        
        GameId = gameId;
    }

    /// <summary>
    /// Starts the game simulation.
    /// </summary>
    public void StartSimulation()
    {
        if (Status != SessionStatus.Created)
        {
            throw new InvalidSessionStateException(string.Format(SessionConstants.ErrorMessages.CannotStartSimulation, Status));
        }
        
        Status = SessionStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        
        _domainEvents.Add(new SimulationStartedEvent(Id));
    }

    /// <summary>
    /// Records a move in the session.
    /// </summary>
    /// <param name="position">The position of the move.</param>
    /// <param name="player">The player making the move.</param>
    public void RecordMove(Position position, Player player)
    {
        if (Status != SessionStatus.InProgress)
        {
            throw new InvalidSessionStateException(string.Format(SessionConstants.ErrorMessages.CannotRecordMoves, Status));
        }
        
        var move = new Move(Id, player, position, MoveType.Random, _moves.Count + 1);
        _moves.Add(move);
        _domainEvents.Add(new MoveMadeEvent(Id, move));
    }

    /// <summary>
    /// Adds a move to the session.
    /// </summary>
    /// <param name="move">The move to add.</param>
    public void AddMove(Move move)
    {
        // Change this check to be more specific
        if (Status != SessionStatus.InProgress)
        {
            throw new InvalidSessionStateException(string.Format(SessionConstants.ErrorMessages.CannotAddMoves, Status));
        }
        
        if (move.SessionId != Id)
        {
            throw new ArgumentException(SessionConstants.ErrorMessages.MoveDoesNotBelongToSession);
        }
        
        _moves.Add(move);
        _domainEvents.Add(new MoveMadeEvent(Id, move));
    }

    /// <summary>
    /// Completes the game session.
    /// </summary>
    /// <param name="result">The result of the game.</param>
    public void Complete(GameStatus result)
    {
        if (Status == SessionStatus.Completed)
        {
            throw new InvalidSessionStateException(SessionConstants.ErrorMessages.SessionAlreadyCompleted);
        }
        
        Status = SessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Result = result;
        
        _domainEvents.Add(new GameCompletedEvent(Id, result));
    }

    /// <summary>
    /// Completes the game session with a winner.
    /// </summary>
    /// <param name="winner">The winner of the game.</param>
    public void CompleteGame(string? winner)
    {
        if (Status == SessionStatus.Completed)
        {
            throw new InvalidSessionStateException(SessionConstants.ErrorMessages.SessionAlreadyCompleted);
        }
        
        Status = SessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Winner = winner;
        
        var result = winner switch
        {
            "X" => GameStatus.Win,
            "O" => GameStatus.Win,
            _ => GameStatus.Draw
        };
        
        Result = result;
        _domainEvents.Add(new GameCompletedEvent(Id, result));
    }

    /// <summary>
    /// Marks the simulation as failed.
    /// </summary>
    public void FailSimulation()
    {
        Status = SessionStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Clears all domain events.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Simulates a complete game using the provided dependencies.
    /// </summary>
    /// <param name="gameEngineClient">The game engine API client.</param>
    /// <param name="moveGenerator">The move generator.</param>
    /// <param name="moveStrategy">The move strategy to use.</param>
    /// <returns>The list of moves made during the simulation.</returns>
    public async Task<List<MoveInfo>> SimulateAsync(
        IGameEngineApiClient gameEngineClient, 
        IMoveGenerator moveGenerator,
        MoveType moveStrategy = MoveType.Random)
    {
        if (Status != SessionStatus.Created)
        {
            throw new InvalidSessionStateException(string.Format(SessionConstants.ErrorMessages.CannotStartSimulation, Status));
        }

        try
        {
            // Start simulation
            StartSimulation();

            // Create game in Game Engine
            var createGameResponse = await gameEngineClient.CreateGameAsync();
            SetGameId(createGameResponse.GameId);
            
            // Simulate moves until game is complete
            var moves = new List<MoveInfo>();
            var currentPlayer = Player.X;
            
            while (Status == SessionStatus.InProgress)
            {
                // Get current game state from Game Engine
                var gameState = await gameEngineClient.GetGameStateAsync(GameId);
                var board = Board.FromStringRepresentation(gameState.Board);
                
                // Generate move using the selected strategy
                var position = moveGenerator.GenerateMove(currentPlayer, board);
                
                // Make move in Game Engine
                var moveRequest = new MakeMoveRequest(position.Row, position.Column);
                gameState = await gameEngineClient.MakeMoveAsync(GameId, moveRequest);
                
                // Record move in session
                RecordMove(position, currentPlayer);
                
                moves.Add(new MoveInfo(position.Row, position.Column, currentPlayer.ToString()));

                // Check if game is complete
                if (gameState.Status == SessionConstants.Status.Completed)
                {
                    CompleteGame(gameState.Winner);
                    break;
                }

                // Switch players
                currentPlayer = currentPlayer == Player.X ? Player.O : Player.X;
            }

            return moves;
        }
        catch (Exception)
        {
            // Handle any errors during simulation
            FailSimulation();
            throw;
        }
    }

    // Private constructor for EF Core
    private GameSession() { }
} 