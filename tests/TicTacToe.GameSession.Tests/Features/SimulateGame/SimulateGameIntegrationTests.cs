using Xunit;
using FluentAssertions;
using System.Text.Json;
using System.Text;
using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.SimulateGame;

public class SimulateGameIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task SimulateGame_WithNonExistentSession_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/sessions/{nonExistentId}/simulate", content);
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode);
    }
    
    [Fact]
    public async Task SimulateGame_WithAlreadyCompletedSession_ShouldReturn400()
    {
        // Arrange
        var session = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        session.StartSimulation();
        session.CompleteGame("X");
        await fixture.GameSessionRepository.SaveAsync(session);
        
        // Act
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/sessions/{session.Id}/simulate", content);
        
        // Assert
        Assert.Equal(400, (int)response.StatusCode);
    }
    
    [Fact]
    public async Task SimulateGame_WithInProgressSession_ShouldReturn400()
    {
        // Arrange
        var session = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        session.StartSimulation();
        await fixture.GameSessionRepository.SaveAsync(session);
        
        // Act
        var request = new { };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/sessions/{session.Id}/simulate", content);
        
        // Assert
        Assert.Equal(400, (int)response.StatusCode);
    }
} 