using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Infrastructure.External;

/// <summary>
/// HTTP client implementation for communicating with the Game Engine Service.
/// </summary>
public class GameEngineHttpClient : IGameEngineApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new Game Engine HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use.</param>
    public GameEngineHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Creates a new game in the Game Engine Service.
    /// </summary>
    /// <returns>The created game response.</returns>
    public async Task<CreateGameResponse> CreateGameAsync()
    {
        var response = await _httpClient.PostAsync("/games", null);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<CreateGameResponse>(_jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize create game response");
    }

    /// <summary>
    /// Makes a move in the specified game.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="request">The move request.</param>
    /// <returns>The updated game state.</returns>
    public async Task<GameStateResponse> MakeMoveAsync(Guid gameId, MakeMoveRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/games/{gameId}/move", request, _jsonOptions);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<GameStateResponse>(_jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize game state response");
    }

    /// <summary>
    /// Gets the current state of a game.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>The current game state.</returns>
    public async Task<GameStateResponse> GetGameStateAsync(Guid gameId)
    {
        var response = await _httpClient.GetAsync($"/games/{gameId}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<GameStateResponse>(_jsonOptions) 
               ?? throw new InvalidOperationException("Failed to deserialize game state response");
    }
} 