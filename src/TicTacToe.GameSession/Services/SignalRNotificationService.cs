using Microsoft.AspNetCore.SignalR;
using TicTacToe.GameSession.Hubs;

namespace TicTacToe.GameSession.Services;

/// <summary>
/// Implementation of SignalR notification service.
/// </summary>
public class SignalRNotificationService : ISignalRNotificationService
{
    private readonly IHubContext<GameHub> _hubContext;

    public SignalRNotificationService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyGameStateUpdatedAsync(string sessionId, object gameState)
    {
        await _hubContext.Clients.Group(sessionId).SendAsync("GameStateUpdated", gameState);
    }

    public async Task NotifyMoveReceivedAsync(string sessionId, object move)
    {
        await _hubContext.Clients.Group(sessionId).SendAsync("MoveReceived", move);
    }

    public async Task NotifyGameCompletedAsync(string sessionId, object finalState)
    {
        await _hubContext.Clients.Group(sessionId).SendAsync("GameCompleted", finalState);
    }

    public async Task NotifyErrorAsync(string sessionId, string errorMessage)
    {
        await _hubContext.Clients.Group(sessionId).SendAsync("Error", errorMessage);
    }
} 