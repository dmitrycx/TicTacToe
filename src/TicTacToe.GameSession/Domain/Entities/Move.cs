using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;

namespace TicTacToe.GameSession.Domain.Entities;

/// <summary>
/// Represents a move made in a game session.
/// </summary>
public class Move
{
    public Guid Id { get; private set; }
    
    public Guid SessionId { get; private set; }
    
    public Player Player { get; private set; }
    
    public Position Position { get; private set; } = null!;
    
    public MoveType Type { get; private set; }
    
    public DateTime MadeAt { get; private set; }
    
    public int MoveNumber { get; private set; }

    /// <summary>
    /// Creates a new move.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="player">The player making the move.</param>
    /// <param name="position">The position of the move.</param>
    /// <param name="type">The type of move.</param>
    /// <param name="moveNumber">The move number in sequence.</param>
    public Move(Guid sessionId, Player player, Position position, MoveType type, int moveNumber)
    {
        Id = Guid.NewGuid();
        SessionId = sessionId;
        Player = player;
        Position = position;
        Type = type;
        MoveNumber = moveNumber;
        MadeAt = DateTime.UtcNow;
    }
} 