using TicTacToe.GameSession.UnitTests.TestHelpers;

namespace TicTacToe.GameSession.UnitTests.Domain.Events;

public class DomainEventTests
{
    [Fact]
    public void SessionCreatedEvent_ShouldHaveCorrectProperties()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        // Act
        var domainEvent = new SessionCreatedEvent(sessionId, gameId);

        // Assert
        Assert.Equal(sessionId, domainEvent.SessionId);
        Assert.Equal(gameId, domainEvent.GameId);
    }

    [Fact]
    public void SimulationStartedEvent_ShouldHaveCorrectProperties()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var domainEvent = new SimulationStartedEvent(sessionId);

        // Assert
        Assert.Equal(sessionId, domainEvent.SessionId);
    }

    [Fact]
    public void MoveMadeEvent_ShouldHaveCorrectProperties()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var move = MoveFactory.CreateXMove(sessionId, 1, 1);

        // Act
        var domainEvent = new MoveMadeEvent(sessionId, move);

        // Assert
        Assert.Equal(sessionId, domainEvent.SessionId);
        Assert.Equal(move, domainEvent.Move);
    }

    [Fact]
    public void GameCompletedEvent_ShouldHaveCorrectProperties()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var result = GameStatus.Win;

        // Act
        var domainEvent = new GameCompletedEvent(sessionId, result);

        // Assert
        Assert.Equal(sessionId, domainEvent.SessionId);
        Assert.Equal(result, domainEvent.Result);
    }

    [Fact]
    public void SessionCreatedEvent_ShouldImplementIDomainEvent()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        // Act
        var domainEvent = new SessionCreatedEvent(sessionId, gameId);

        // Assert
        Assert.IsAssignableFrom<IDomainEvent>(domainEvent);
    }

    [Fact]
    public void SimulationStartedEvent_ShouldImplementIDomainEvent()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var domainEvent = new SimulationStartedEvent(sessionId);

        // Assert
        Assert.IsAssignableFrom<IDomainEvent>(domainEvent);
    }

    [Fact]
    public void MoveMadeEvent_ShouldImplementIDomainEvent()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var move = MoveFactory.CreateXMove(sessionId, 1, 1);

        // Act
        var domainEvent = new MoveMadeEvent(sessionId, move);

        // Assert
        Assert.IsAssignableFrom<IDomainEvent>(domainEvent);
    }

    [Fact]
    public void GameCompletedEvent_ShouldImplementIDomainEvent()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var result = GameStatus.Win;

        // Act
        var domainEvent = new GameCompletedEvent(sessionId, result);

        // Assert
        Assert.IsAssignableFrom<IDomainEvent>(domainEvent);
    }

    [Fact]
    public void SessionCreatedEvent_ShouldSetOccurredOnToCurrentTime_WhenNotProvided()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var domainEvent = new SessionCreatedEvent(sessionId, gameId);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(domainEvent.OccurredOn >= beforeCreation);
        Assert.True(domainEvent.OccurredOn <= afterCreation);
    }

    [Fact]
    public void SimulationStartedEvent_ShouldSetOccurredOnToCurrentTime_WhenNotProvided()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var domainEvent = new SimulationStartedEvent(sessionId);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(domainEvent.OccurredOn >= beforeCreation);
        Assert.True(domainEvent.OccurredOn <= afterCreation);
    }

    [Fact]
    public void MoveMadeEvent_ShouldSetOccurredOnToCurrentTime_WhenNotProvided()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var move = MoveFactory.CreateXMove(sessionId, 1, 1);
        var beforeCreation = DateTime.UtcNow;

        // Act
        var domainEvent = new MoveMadeEvent(sessionId, move);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(domainEvent.OccurredOn >= beforeCreation);
        Assert.True(domainEvent.OccurredOn <= afterCreation);
    }

    [Fact]
    public void GameCompletedEvent_ShouldSetOccurredOnToCurrentTime_WhenNotProvided()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var result = GameStatus.Win;
        var beforeCreation = DateTime.UtcNow;

        // Act
        var domainEvent = new GameCompletedEvent(sessionId, result);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(domainEvent.OccurredOn >= beforeCreation);
        Assert.True(domainEvent.OccurredOn <= afterCreation);
    }
} 