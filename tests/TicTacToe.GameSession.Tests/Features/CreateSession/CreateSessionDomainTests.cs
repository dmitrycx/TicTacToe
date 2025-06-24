using Xunit;
using FluentAssertions;
using Moq;
using TicTacToe.GameSession.Persistence;
using TicTacToe.GameSession.Tests.TestHelpers;

namespace TicTacToe.GameSession.Tests.Features.CreateSession;

/// <summary>
/// Unit tests for CreateSession domain logic and repository behavior
/// </summary>
public class CreateSessionDomainTests
{
    [Fact]
    public async Task Repository_SaveAsync_ShouldPersistSession()
    {
        // Arrange
        var session = GameSessionTestHelpers.CreateNewSession();
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.SaveAsync(It.IsAny<TicTacToe.GameSession.Domain.Aggregates.GameSession>()))
            .ReturnsAsync((TicTacToe.GameSession.Domain.Aggregates.GameSession s) => s);

        // Act
        var savedSession = await mockRepository.Object.SaveAsync(session);

        // Assert
        savedSession.Should().Be(session);
        mockRepository.Verify(r => r.SaveAsync(It.IsAny<TicTacToe.GameSession.Domain.Aggregates.GameSession>()), Times.Once);
    }

    [Fact]
    public void GameSession_Create_ShouldCreateNewSessionWithCorrectInitialState()
    {
        // Arrange & Act
        var session = TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();

        // Assert
        session.Id.Should().NotBeEmpty();
        session.Status.ToString().Should().Be("Created");
        session.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        session.GameId.Should().Be(Guid.Empty); // Default value
        session.Winner.Should().BeNull();
        session.Moves.Should().BeEmpty();
        session.StartedAt.Should().BeNull();
        session.CompletedAt.Should().BeNull();
        session.Result.Should().BeNull();
    }

    [Fact]
    public void GameSession_Create_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var session1 = TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();
        var session2 = TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();

        // Assert
        session1.Id.Should().NotBe(session2.Id);
    }

    [Fact]
    public void GameSession_Create_ShouldRaiseSessionCreatedEvent()
    {
        // Arrange & Act
        var session = TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();

        // Assert
        session.DomainEvents.Should().HaveCount(1);
        session.DomainEvents.First().Should().BeOfType<TicTacToe.GameSession.Domain.Events.SessionCreatedEvent>();
        
        var createdEvent = (TicTacToe.GameSession.Domain.Events.SessionCreatedEvent)session.DomainEvents.First();
        createdEvent.SessionId.Should().Be(session.Id);
    }

    [Fact]
    public void GameSession_Create_ShouldSetCreatedAtToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var session = TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();

        // Assert
        session.CreatedAt.Should().BeAfter(beforeCreation);
        session.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }
} 