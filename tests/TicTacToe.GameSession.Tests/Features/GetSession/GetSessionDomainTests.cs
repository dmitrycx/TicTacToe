using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameSession.Endpoints;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

/// <summary>
/// Unit tests for GetSession domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class GetSessionDomainTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ToResponse_ShouldMapAllPropertiesCorrectly_ForCompletedSession()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var session = new Domain.Aggregates.GameSession(gameId);
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
        response.Should().NotBeNull();
        response.SessionId.Should().Be(session.Id);
        response.CurrentGameId.Should().Be(session.CurrentGameId);
        response.Status.Should().Be(session.Status.ToString());
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
    [Trait("Category", "Unit")]
    public void ToResponse_ShouldHandleNullsCorrectly_ForNewSession()
    {
        // Arrange
        var session = new Domain.Aggregates.GameSession(Guid.NewGuid());
        
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

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetByIdAsync_ShouldReturnSession_WhenSessionExists()
    {
        // Arrange
        var session = Domain.Aggregates.GameSession.Create();
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(session.Id))
            .ReturnsAsync(session);

        // Act
        var retrievedSession = await mockRepository.Object.GetByIdAsync(session.Id);

        // Assert
        retrievedSession.Should().Be(session);
        mockRepository.Verify(r => r.GetByIdAsync(session.Id), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetByIdAsync_ShouldReturnNull_WhenSessionDoesNotExist()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Domain.Aggregates.GameSession?)null);

        // Act
        var retrievedSession = await mockRepository.Object.GetByIdAsync(sessionId);

        // Assert
        retrievedSession.Should().BeNull();
        mockRepository.Verify(r => r.GetByIdAsync(sessionId), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GameSession_Properties_ShouldBeAccessible()
    {
        // Arrange
        var session = Domain.Aggregates.GameSession.Create();

        // Act & Assert
        session.Id.Should().NotBeEmpty();
        session.Status.Should().Be(SessionStatus.Created);
        session.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        session.Moves.Should().BeEmpty();
    }
} 