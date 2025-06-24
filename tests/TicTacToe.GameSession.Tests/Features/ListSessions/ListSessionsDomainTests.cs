using Xunit;
using FluentAssertions;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameSession.Domain.Entities;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Tests.Features.ListSessions;

public class ListSessionsDomainTests
{
    [Fact]
    public void ToResponse_ShouldMapMultipleSessionsCorrectly()
    {
        // Arrange
        var sessions = new List<TicTacToe.GameSession.Domain.Aggregates.GameSession>();
        
        // Session 1: Created
        var session1 = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        sessions.Add(session1);
        
        // Session 2: In Progress with moves
        var session2 = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        session2.StartSimulation();
        session2.RecordMove(new Position(0, 0), Player.X);
        session2.RecordMove(new Position(1, 1), Player.O);
        sessions.Add(session2);
        
        // Session 3: Completed with winner
        var session3 = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        session3.StartSimulation();
        session3.RecordMove(new Position(0, 0), Player.X);
        session3.RecordMove(new Position(0, 1), Player.O);
        session3.RecordMove(new Position(0, 2), Player.X);
        session3.CompleteGame("X");
        sessions.Add(session3);
        
        // Act
        var response = sessions.ToResponse();
        
        // Assert
        response.Sessions.Should().HaveCount(3);
        
        // Verify session 1 (Created)
        var summary1 = response.Sessions[0];
        summary1.SessionId.Should().Be(session1.Id);
        summary1.Status.Should().Be("Created");
        summary1.MoveCount.Should().Be(0);
        summary1.Winner.Should().BeNull();
        
        // Verify session 2 (In Progress)
        var summary2 = response.Sessions[1];
        summary2.SessionId.Should().Be(session2.Id);
        summary2.Status.Should().Be("InProgress");
        summary2.MoveCount.Should().Be(2);
        summary2.Winner.Should().BeNull();
        
        // Verify session 3 (Completed)
        var summary3 = response.Sessions[2];
        summary3.SessionId.Should().Be(session3.Id);
        summary3.Status.Should().Be("Completed");
        summary3.MoveCount.Should().Be(3);
        summary3.Winner.Should().Be("X");
    }
    
    [Fact]
    public void ToResponse_ShouldHandleEmptyList()
    {
        // Arrange
        var sessions = new List<TicTacToe.GameSession.Domain.Aggregates.GameSession>();
        
        // Act
        var response = sessions.ToResponse();
        
        // Assert
        response.Sessions.Should().BeEmpty();
    }
    
    [Fact]
    public void ToSummary_ShouldMapIndividualSessionCorrectly()
    {
        // Arrange
        var session = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        session.StartSimulation();
        session.RecordMove(new Position(0, 0), Player.X);
        session.CompleteGame("X");
        
        // Act
        var summary = session.ToSummary();
        
        // Assert
        summary.SessionId.Should().Be(session.Id);
        summary.Status.Should().Be("Completed");
        summary.CreatedAt.Should().Be(session.CreatedAt);
        summary.MoveCount.Should().Be(1);
        summary.Winner.Should().Be("X");
    }
} 