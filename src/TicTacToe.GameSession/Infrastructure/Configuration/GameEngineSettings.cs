using System.ComponentModel.DataAnnotations;

namespace TicTacToe.GameSession.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for the Game Engine Service.
/// </summary>
public class GameEngineSettings
{
    /// <summary>
    /// The base URL of the Game Engine Service.
    /// </summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = "http://localhost:5000";
    
    /// <summary>
    /// Timeout for HTTP requests in seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Maximum number of retry attempts for failed requests.
    /// </summary>
    [Range(0, 10)]
    public int MaxRetries { get; set; } = 3;
} 