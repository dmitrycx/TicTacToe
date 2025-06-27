using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.GameSession.Hubs;

public class GameHub : Hub
{
    private readonly IGameSessionRepository _gameSessionRepository;

    public GameHub(IGameSessionRepository gameSessionRepository)
    {
        _gameSessionRepository = gameSessionRepository;
    }

    public async Task JoinGameSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Caller.SendAsync("JoinedSession", sessionId);
    }

    public async Task LeaveGameSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
    }

    // Method to notify clients about game state updates
    public async Task NotifyGameStateUpdated(string sessionId, object gameState)
    {
        await Clients.Group(sessionId).SendAsync("GameStateUpdated", gameState);
    }

    // Method to notify clients about moves
    public async Task NotifyMoveReceived(string sessionId, object move)
    {
        await Clients.Group(sessionId).SendAsync("MoveReceived", move);
    }

    // Method to notify clients about game completion
    public async Task NotifyGameCompleted(string sessionId, object finalState)
    {
        await Clients.Group(sessionId).SendAsync("GameCompleted", finalState);
    }

    // Method to notify clients about errors
    public async Task NotifyError(string sessionId, string errorMessage)
    {
        await Clients.Group(sessionId).SendAsync("Error", errorMessage);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
    }
} 