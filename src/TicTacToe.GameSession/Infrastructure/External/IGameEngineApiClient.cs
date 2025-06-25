using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Infrastructure.External;

/// <summary>
/// Interface for communicating with the Game Engine Service API.
/// </summary>
public interface IGameEngineApiClient
{
    /// <summary>
    /// Creates a new game in the Game Engine Service.
    /// </summary>
    /// <returns>The created game response.</returns>
    Task<CreateGameResponse> CreateGameAsync();
    
    /// <summary>
    /// Makes a move in the specified game.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="request">The move request.</param>
    /// <returns>The updated game state.</returns>
    Task<GameStateResponse> MakeMoveAsync(Guid gameId, MakeMoveRequest request);
    
    /// <summary>
    /// Gets the current state of a game.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>The current game state.</returns>
    Task<GameStateResponse> GetGameStateAsync(Guid gameId);
} 