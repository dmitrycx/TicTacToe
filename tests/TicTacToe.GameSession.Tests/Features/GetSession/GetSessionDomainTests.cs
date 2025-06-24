using Xunit;
using FluentAssertions;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameSession.Domain.Entities;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

public class GetSessionDomainTests
{
    [Fact]
    public void ToResponse_ShouldMapAllPropertiesCorrectly_ForCompletedSession()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var session = new TicTacToe.GameSession.Domain.Aggregates.GameSession(gameId);
        var createdAt = session.CreatedAt;
        session.StartSimulation();
        var startedAt = session.StartedAt;
        session.RecordMove(new Position(0, 0), Player.X);
        session.RecordMove(new Position(1, 1), Player.O);
        session.RecordMove(new Position(2, 2), Player.X);
        session.CompleteGame("X");
        var completedAt = session.CompletedAt;
        
        // Act
        var response = session.ToResponse();
        
        // Assert
        response.SessionId.Should().Be(session.Id);
        response.GameId.Should().Be(gameId);
        response.Status.Should().Be("Completed");
        response.CreatedAt.Should().Be(createdAt);
        response.StartedAt.Should().Be(startedAt);
        response.CompletedAt.Should().Be(completedAt);
        response.Winner.Should().Be("X");
        response.Result.Should().Be("Win");
        response.Moves.Should().HaveCount(3);
        
        // Verify moves are mapped correctly
        var moves = response.Moves.ToList();
        moves[0].Row.Should().Be(0);
        moves[0].Column.Should().Be(0);
        moves[0].Player.Should().Be("X");
        moves[1].Row.Should().Be(1);
        moves[1].Column.Should().Be(1);
        moves[1].Player.Should().Be("O");
        moves[2].Row.Should().Be(2);
        moves[2].Column.Should().Be(2);
        moves[2].Player.Should().Be("X");
    }
    
    [Fact]
    public void ToResponse_ShouldHandleNullsCorrectly_ForNewSession()
    {
        // Arrange
        var session = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        
        // Act
        var response = session.ToResponse();
        
        // Assert
        response.Status.Should().Be("Created");
        response.StartedAt.Should().BeNull();
        response.CompletedAt.Should().BeNull();
        response.Winner.Should().BeNull();
        response.Result.Should().BeNull();
        response.Moves.Should().BeEmpty();
    }
} 