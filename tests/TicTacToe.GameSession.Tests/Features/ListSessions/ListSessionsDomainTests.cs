using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Tests.Features.ListSessions;

/// <summary>
/// Unit tests for ListSessions domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class ListSessionsDomainTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ToResponse_ShouldMapMultipleSessionsCorrectly()
    {
        // Arrange
        var sessions = new List<Domain.Aggregates.GameSession>();
        
        // Session 1: Created
        var session1 = new Domain.Aggregates.GameSession(Guid.NewGuid());
        sessions.Add(session1);
        
        // Session 2: In Progress with moves
        var session2 = new Domain.Aggregates.GameSession(Guid.NewGuid());
        session2.StartSimulation();
        session2.RecordMove(Position.Create(0, 0), Player.X);
        session2.RecordMove(Position.Create(1, 1), Player.O);
        sessions.Add(session2);
        
        // Session 3: Completed with winner
        var session3 = new Domain.Aggregates.GameSession(Guid.NewGuid());
        session3.StartSimulation();
        session3.RecordMove(Position.Create(0, 0), Player.X);
        session3.RecordMove(Position.Create(0, 1), Player.O);
        session3.RecordMove(Position.Create(0, 2), Player.X);
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
    [Trait("Category", "Unit")]
    public void ToResponse_ShouldHandleEmptyList()
    {
        // Arrange
        var sessions = new List<Domain.Aggregates.GameSession>();
        
        // Act
        var response = sessions.ToResponse();
        
        // Assert
        response.Sessions.Should().BeEmpty();
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void ToSummary_ShouldMapIndividualSessionCorrectly()
    {
        // Arrange
        var session = new Domain.Aggregates.GameSession(Guid.NewGuid());
        session.StartSimulation();
        session.RecordMove(Position.Create(0, 0), Player.X);
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

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetAllAsync_ShouldReturnAllSessions()
    {
        // Arrange
        var sessions = new List<Domain.Aggregates.GameSession>
        {
            Domain.Aggregates.GameSession.Create(),
            Domain.Aggregates.GameSession.Create(),
            Domain.Aggregates.GameSession.Create()
        };
        
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(sessions);

        // Act
        var retrievedSessions = await mockRepository.Object.GetAllAsync();

        // Assert
        retrievedSessions.Should().BeEquivalentTo(sessions);
        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetAllAsync_ShouldReturnEmptyList_WhenNoSessionsExist()
    {
        // Arrange
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Domain.Aggregates.GameSession>());

        // Act
        var retrievedSessions = await mockRepository.Object.GetAllAsync();

        // Assert
        retrievedSessions.Should().BeEmpty();
        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
} 