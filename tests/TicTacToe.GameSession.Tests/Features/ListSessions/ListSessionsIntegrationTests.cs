using Xunit;
using FluentAssertions;
using System.Text.Json;
using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.ListSessions;

public class ListSessionsIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task ListSessions_ShouldReturnMultipleSessions_WhenSessionsExist()
    {
        // Arrange
        var session1 = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        var session2 = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        var session3 = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        
        // Add some state to sessions
        session2.StartSimulation();
        session2.RecordMove(new TicTacToe.GameEngine.Domain.ValueObjects.Position(0, 0), TicTacToe.GameEngine.Domain.Enums.Player.X);
        
        session3.StartSimulation();
        session3.RecordMove(new TicTacToe.GameEngine.Domain.ValueObjects.Position(0, 0), TicTacToe.GameEngine.Domain.Enums.Player.X);
        session3.CompleteGame("X");
        
        await fixture.GameSessionRepository.SaveAsync(session1);
        await fixture.GameSessionRepository.SaveAsync(session2);
        await fixture.GameSessionRepository.SaveAsync(session3);
        
        // Act
        var response = await _client.GetAsync("/sessions");
        var content = await response.Content.ReadAsStringAsync();
        var sessionsResponse = JsonSerializer.Deserialize<ListSessionsResponse>(content, _jsonOptions);
        
        // Assert
        Assert.Equal(200, (int)response.StatusCode);
        Assert.NotNull(sessionsResponse);
        sessionsResponse.Sessions.Should().HaveCount(3);
        
        // Verify all sessions are present
        var sessionIds = sessionsResponse.Sessions.Select(s => s.SessionId).ToList();
        sessionIds.Should().Contain(session1.Id);
        sessionIds.Should().Contain(session2.Id);
        sessionIds.Should().Contain(session3.Id);
        
        // Verify session states
        var session1Summary = sessionsResponse.Sessions.First(s => s.SessionId == session1.Id);
        session1Summary.Status.Should().Be("Created");
        session1Summary.MoveCount.Should().Be(0);
        session1Summary.Winner.Should().BeNull();
        
        var session2Summary = sessionsResponse.Sessions.First(s => s.SessionId == session2.Id);
        session2Summary.Status.Should().Be("InProgress");
        session2Summary.MoveCount.Should().Be(1);
        session2Summary.Winner.Should().BeNull();
        
        var session3Summary = sessionsResponse.Sessions.First(s => s.SessionId == session3.Id);
        session3Summary.Status.Should().Be("Completed");
        session3Summary.MoveCount.Should().Be(1);
        session3Summary.Winner.Should().Be("X");
    }
    
    [Fact]
    public async Task ListSessions_ShouldReturnEmptyList_WhenNoSessionsExist()
    {
        // Arrange - ensure clean repository
        var allSessions = await fixture.GameSessionRepository.GetAllAsync();
        foreach (var session in allSessions)
        {
            await fixture.GameSessionRepository.DeleteAsync(session.Id);
        }
        
        // Act
        var response = await _client.GetAsync("/sessions");
        var content = await response.Content.ReadAsStringAsync();
        var sessionsResponse = JsonSerializer.Deserialize<ListSessionsResponse>(content, _jsonOptions);
        
        // Assert
        Assert.Equal(200, (int)response.StatusCode);
        Assert.NotNull(sessionsResponse);
        sessionsResponse.Sessions.Should().BeEmpty();
    }
} 