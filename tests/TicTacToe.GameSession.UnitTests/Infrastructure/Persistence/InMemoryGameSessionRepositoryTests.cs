using TicTacToe.GameSession.UnitTests.TestHelpers;

namespace TicTacToe.GameSession.UnitTests.Infrastructure.Persistence;

public class InMemoryGameSessionRepositoryTests
{
    private readonly InMemoryGameSessionRepository _repository = new();

    [Fact]
    public async Task SaveAsync_WithNewSession_ShouldAddSession()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();

        // Act
        var savedSession = await _repository.SaveAsync(session);

        // Assert
        Assert.Equal(session.Id, savedSession.Id);
        var retrievedSession = await _repository.GetByIdAsync(session.Id);
        Assert.NotNull(retrievedSession);
        Assert.Equal(session.Id, retrievedSession!.Id);
    }

    [Fact]
    public async Task SaveAsync_WithExistingSession_ShouldUpdateSession()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();
        await _repository.SaveAsync(session);
        
        // Modify the session
        session.StartSimulation();

        // Act
        var savedSession = await _repository.SaveAsync(session);

        // Assert
        Assert.Equal(session.Id, savedSession.Id);
        var retrievedSession = await _repository.GetByIdAsync(session.Id);
        Assert.NotNull(retrievedSession);
        Assert.Equal(SessionStatus.InProgress, retrievedSession!.Status);
    }

    [Fact]
    public async Task SaveAsync_WithNullSession_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(null!));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingSession_ShouldReturnSession()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();
        await _repository.SaveAsync(session);

        // Act
        var retrievedSession = await _repository.GetByIdAsync(session.Id);

        // Assert
        Assert.NotNull(retrievedSession);
        Assert.Equal(session.Id, retrievedSession!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentSession_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var retrievedSession = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(retrievedSession);
    }

    [Fact]
    public async Task GetAllAsync_WithNoSessions_ShouldReturnEmptyCollection()
    {
        // Act
        var sessions = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(sessions);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleSessions_ShouldReturnAllSessions()
    {
        // Arrange
        var session1 = GameSessionFactory.CreateCreatedSession();
        var session2 = GameSessionFactory.CreateInProgressSession();
        var session3 = GameSessionFactory.CreateCompletedSession();

        await _repository.SaveAsync(session1);
        await _repository.SaveAsync(session2);
        await _repository.SaveAsync(session3);

        // Act
        var sessions = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, sessions.Count());
        Assert.Contains(sessions, s => s.Id == session1.Id);
        Assert.Contains(sessions, s => s.Id == session2.Id);
        Assert.Contains(sessions, s => s.Id == session3.Id);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingSession_ShouldRemoveSession()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();
        await _repository.SaveAsync(session);

        // Act
        var deleted = await _repository.DeleteAsync(session.Id);

        // Assert
        Assert.True(deleted);
        var retrievedSession = await _repository.GetByIdAsync(session.Id);
        Assert.Null(retrievedSession);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentSession_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var deleted = await _repository.DeleteAsync(nonExistentId);

        // Assert
        Assert.False(deleted);
    }

    [Fact]
    public async Task Repository_IsolationBetweenInstances()
    {
        // Arrange
        var repository1 = new InMemoryGameSessionRepository();
        var repository2 = new InMemoryGameSessionRepository();
        var session = GameSessionFactory.CreateCreatedSession();

        // Act
        await repository1.SaveAsync(session);

        // Assert
        var sessionInRepo1 = await repository1.GetByIdAsync(session.Id);
        var sessionInRepo2 = await repository2.GetByIdAsync(session.Id);

        Assert.NotNull(sessionInRepo1);
        Assert.Null(sessionInRepo2);
    }
} 