namespace TicTacToe.GameSession.Tests.Features.CreateSession;

/// <summary>
/// Unit tests for CreateSession domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class CreateSessionDomainTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void GameSession_Create_ShouldInitializeWithCorrectState()
    {
        // Act
        var session = Domain.Aggregates.GameSession.Create();

        // Assert
        session.Id.Should().NotBeEmpty();
        session.CurrentGameId.Should().Be(Guid.Empty);
        session.Status.Should().Be(SessionStatus.Created);
        session.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        session.Moves.Should().BeEmpty();
        session.Winner.Should().BeNull();
        session.StartedAt.Should().BeNull();
        session.CompletedAt.Should().BeNull();
        session.Result.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GameSession_Create_ShouldGenerateUniqueIds_WhenMultipleSessionsCreated()
    {
        // Act
        var session1 = Domain.Aggregates.GameSession.Create();
        var session2 = Domain.Aggregates.GameSession.Create();

        // Assert
        session1.Id.Should().NotBe(session2.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GameSession_Create_ShouldRaiseSessionCreatedEvent()
    {
        // Arrange & Act
        var session = Domain.Aggregates.GameSession.Create();

        // Assert
        session.DomainEvents.Should().HaveCount(1);
        session.DomainEvents.First().Should().BeOfType<SessionCreatedEvent>();
        
        var createdEvent = (SessionCreatedEvent)session.DomainEvents.First();
        createdEvent.SessionId.Should().Be(session.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GameSession_Create_ShouldSetCreatedAtToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var session = Domain.Aggregates.GameSession.Create();

        // Assert
        session.CreatedAt.Should().BeAfter(beforeCreation);
        session.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_SaveAsync_ShouldPersistSession()
    {
        // Arrange
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.SaveAsync(It.IsAny<Domain.Aggregates.GameSession>()))
            .ReturnsAsync((Domain.Aggregates.GameSession session) => session);
        
        var session = Domain.Aggregates.GameSession.Create();

        // Act
        var savedSession = await mockRepository.Object.SaveAsync(session);

        // Assert
        savedSession.Should().Be(session);
        mockRepository.Verify(r => r.SaveAsync(It.IsAny<Domain.Aggregates.GameSession>()), Times.Once);
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
} 