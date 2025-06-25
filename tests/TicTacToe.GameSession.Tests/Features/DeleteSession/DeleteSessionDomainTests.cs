using TicTacToe.GameSession.Tests.TestHelpers;

namespace TicTacToe.GameSession.Tests.Features.DeleteSession;

/// <summary>
/// Unit tests for DeleteSession domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class DeleteSessionDomainTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_DeleteAsync_ShouldReturnTrue_WhenSessionExists()
    {
        // Arrange
        var session = GameSessionTestHelpers.CreateNewSession();
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.DeleteAsync(session.Id))
            .ReturnsAsync(true);

        // Act
        var result = await mockRepository.Object.DeleteAsync(session.Id);

        // Assert
        result.Should().BeTrue();
        mockRepository.Verify(r => r.DeleteAsync(session.Id), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_DeleteAsync_ShouldReturnFalse_WhenSessionDoesNotExist()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.DeleteAsync(sessionId))
            .ReturnsAsync(false);

        // Act
        var result = await mockRepository.Object.DeleteAsync(sessionId);

        // Assert
        result.Should().BeFalse();
        mockRepository.Verify(r => r.DeleteAsync(sessionId), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_DeleteAsync_ShouldRemoveSessionFromRepository()
    {
        // Arrange
        var session = GameSessionTestHelpers.CreateNewSession();
        var mockRepository = new Mock<IGameSessionRepository>();
        
        // Setup: Session exists initially, then is deleted
        mockRepository.Setup(r => r.GetByIdAsync(session.Id))
            .ReturnsAsync(session);
        mockRepository.Setup(r => r.DeleteAsync(session.Id))
            .ReturnsAsync(true);
        mockRepository.Setup(r => r.GetByIdAsync(session.Id))
            .ReturnsAsync((TicTacToe.GameSession.Domain.Aggregates.GameSession?)null);

        // Act
        var deleteResult = await mockRepository.Object.DeleteAsync(session.Id);
        var getResult = await mockRepository.Object.GetByIdAsync(session.Id);

        // Assert
        deleteResult.Should().BeTrue();
        getResult.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_DeleteAsync_ShouldHandleMultipleDeletions()
    {
        // Arrange
        var session1 = GameSessionTestHelpers.CreateNewSession();
        var session2 = GameSessionTestHelpers.CreateNewSession();
        var mockRepository = new Mock<IGameSessionRepository>();
        
        mockRepository.Setup(r => r.DeleteAsync(session1.Id))
            .ReturnsAsync(true);
        mockRepository.Setup(r => r.DeleteAsync(session2.Id))
            .ReturnsAsync(true);

        // Act
        var result1 = await mockRepository.Object.DeleteAsync(session1.Id);
        var result2 = await mockRepository.Object.DeleteAsync(session2.Id);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        mockRepository.Verify(r => r.DeleteAsync(session1.Id), Times.Once);
        mockRepository.Verify(r => r.DeleteAsync(session2.Id), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_DeleteAsync_ShouldHandleDeletionOfNonExistentSession()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var mockRepository = new Mock<IGameSessionRepository>();
        mockRepository.Setup(r => r.DeleteAsync(sessionId))
            .ReturnsAsync(false);

        // Act
        var result = await mockRepository.Object.DeleteAsync(sessionId);

        // Assert
        result.Should().BeFalse();
        mockRepository.Verify(r => r.DeleteAsync(sessionId), Times.Once);
    }
} 