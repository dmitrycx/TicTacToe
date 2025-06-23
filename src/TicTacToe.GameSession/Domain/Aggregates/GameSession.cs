using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameSession.Domain.Constants;

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

    // Private constructor for EF Core
    private GameSession() { }
} 