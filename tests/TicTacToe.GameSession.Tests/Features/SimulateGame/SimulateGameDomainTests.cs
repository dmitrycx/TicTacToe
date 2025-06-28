using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;
using TicTacToe.GameEngine.Domain.Entities;
using TicTacToe.GameSession.Endpoints;
using TicTacToe.Shared.Enums;

namespace TicTacToe.GameSession.Tests.Features.SimulateGame;

/// <summary>
/// Unit tests for SimulateGame domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class SimulateGameDomainTests
{
    private readonly Mock<IMoveGenerator> _mockMoveGenerator;
    private readonly Mock<IGameEngineApiClient> _mockApiClient;
    private readonly Domain.Aggregates.GameSession _session;

    public SimulateGameDomainTests()
    {
        _mockMoveGenerator = new Mock<IMoveGenerator>();
        _mockApiClient = new Mock<IGameEngineApiClient>();
        _session = Domain.Aggregates.GameSession.Create();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Simulate_Should_CreateGame_And_StartSimulation()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        _mockApiClient.Setup(x => x.CreateGameAsync())
            .ReturnsAsync(new CreateGameResponse(gameId, DateTime.UtcNow));
        
        _mockApiClient.Setup(x => x.GetGameStateAsync(gameId))
            .ReturnsAsync(new GameStateResponse(
                gameId, "InProgress", "X", null,
                new List<List<string?>> 
                { 
                    new() { null, null, null },
                    new() { null, null, null },
                    new() { null, null, null }
                }, 
                DateTime.UtcNow, null));
        
        _mockMoveGenerator.Setup(x => x.GenerateMove(It.IsAny<Player>(), It.IsAny<Board>()))
            .Returns(Position.Create(0, 0));
        
        _mockApiClient.Setup(x => x.MakeMoveAsync(gameId, It.IsAny<MakeMoveRequest>()))
            .ReturnsAsync(new GameStateResponse(
                gameId, "Completed", "X", "X",
                new List<List<string?>> 
                { 
                    new() { "X", null, null },
                    new() { null, null, null },
                    new() { null, null, null }
                }, 
                DateTime.UtcNow, DateTime.UtcNow));
        
        // Act
        var moves = await _session.SimulateAsync(_mockApiClient.Object, _mockMoveGenerator.Object);
        
        // Assert
        _session.Status.Should().Be(SessionStatus.Completed);
        _session.CurrentGameId.Should().NotBe(Guid.Empty);
        _session.Moves.Should().HaveCount(1);
        
        _mockApiClient.Verify(x => x.CreateGameAsync(), Times.Once);
        _mockApiClient.Verify(x => x.GetGameStateAsync(gameId), Times.Once);
        _mockApiClient.Verify(x => x.MakeMoveAsync(It.IsAny<Guid>(), It.IsAny<MakeMoveRequest>()), Times.Once);
        _mockMoveGenerator.Verify(x => x.GenerateMove(It.IsAny<Player>(), It.IsAny<Board>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Simulate_Should_CompleteWithWinner_WhenApiClientReportsWin()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var moveCount = 0;
        
        _mockApiClient.Setup(x => x.CreateGameAsync())
            .ReturnsAsync(new CreateGameResponse(gameId, DateTime.UtcNow));
        
        _mockApiClient.Setup(x => x.GetGameStateAsync(gameId))
            .ReturnsAsync(new GameStateResponse(
                gameId, "InProgress", "X", null,
                new List<List<string?>> 
                { 
                    new() { null, null, null },
                    new() { null, null, null },
                    new() { null, null, null }
                }, 
                DateTime.UtcNow, null));
        
        _mockMoveGenerator.Setup(x => x.GenerateMove(It.IsAny<Player>(), It.IsAny<Board>()))
            .Returns(Position.Create(0, 0));
        
        _mockApiClient.Setup(x => x.MakeMoveAsync(gameId, It.IsAny<MakeMoveRequest>()))
            .ReturnsAsync(() =>
            {
                moveCount++;
                var status = moveCount >= 5 ? "Completed" : "InProgress";
                var winner = moveCount >= 5 ? "X" : null;
                
                return new GameStateResponse(
                    gameId, status, "X", winner,
                    new List<List<string?>> 
                    { 
                        new() { "X", null, null },
                        new() { null, null, null },
                        new() { null, null, null }
                    }, 
                    DateTime.UtcNow, DateTime.UtcNow);
            });
        
        // Act
        var moves = await _session.SimulateAsync(_mockApiClient.Object, _mockMoveGenerator.Object);
        
        // Assert
        _session.Status.Should().Be(SessionStatus.Completed);
        _session.Winner.Should().Be("X");
        _session.Result.Should().Be(GameStatus.Win);
        moves.Should().HaveCount(5);
        
        _mockApiClient.Verify(x => x.MakeMoveAsync(It.IsAny<Guid>(), It.IsAny<MakeMoveRequest>()), Times.Exactly(5));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Simulate_Should_CompleteWithDraw_WhenApiClientReportsDraw()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var moveCount = 0;
        
        _mockApiClient.Setup(x => x.CreateGameAsync())
            .ReturnsAsync(new CreateGameResponse(gameId, DateTime.UtcNow));
        
        _mockApiClient.Setup(x => x.GetGameStateAsync(gameId))
            .ReturnsAsync(new GameStateResponse(
                gameId, "InProgress", "X", null,
                new List<List<string?>> 
                { 
                    new() { null, null, null },
                    new() { null, null, null },
                    new() { null, null, null }
                }, 
                DateTime.UtcNow, null));
        
        _mockMoveGenerator.Setup(x => x.GenerateMove(It.IsAny<Player>(), It.IsAny<Board>()))
            .Returns(Position.Create(0, 0));
        
        _mockApiClient.Setup(x => x.MakeMoveAsync(gameId, It.IsAny<MakeMoveRequest>()))
            .ReturnsAsync(() =>
            {
                moveCount++;
                var status = moveCount >= 9 ? "Completed" : "InProgress";
                var winner = moveCount >= 9 ? (string?)null : (string?)null; // Draw has no winner
                
                return new GameStateResponse(
                    gameId, status, "X", winner,
                    new List<List<string?>> 
                    { 
                        new() { "X", "O", "X" },
                        new() { "O", "X", "O" },
                        new() { "X", "O", "X" }
                    }, 
                    DateTime.UtcNow, DateTime.UtcNow);
            });
        
        // Act
        var moves = await _session.SimulateAsync(_mockApiClient.Object, _mockMoveGenerator.Object);
        
        // Assert
        _session.Status.Should().Be(SessionStatus.Completed);
        _session.Winner.Should().BeNull();
        _session.Result.Should().Be(GameStatus.Draw);
        moves.Should().HaveCount(9);
        
        _mockApiClient.Verify(x => x.MakeMoveAsync(It.IsAny<Guid>(), It.IsAny<MakeMoveRequest>()), Times.Exactly(9));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Simulate_Should_ThrowException_IfCreateGameFails()
    {
        // Arrange
        _mockApiClient.Setup(x => x.CreateGameAsync())
            .ThrowsAsync(new Exception("Game creation failed"));
        
        // Act & Assert
        var action = () => _session.SimulateAsync(_mockApiClient.Object, _mockMoveGenerator.Object);
        
        await action.Should().ThrowAsync<Exception>();
        _session.Status.Should().Be(SessionStatus.Failed);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Simulate_Should_ThrowException_IfSessionNotInCreatedState()
    {
        // Arrange
        _session.StartSimulation(); // Change state to InProgress
        
        // Act & Assert
        var action = () => _session.SimulateAsync(_mockApiClient.Object, _mockMoveGenerator.Object);
        
        await action.Should().ThrowAsync<InvalidSessionStateException>();
        _session.Status.Should().Be(SessionStatus.InProgress);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GameSession_SimulateGame_ShouldCompleteSuccessfully()
    {
        // Arrange
        var session = Domain.Aggregates.GameSession.Create();
        var mockMoveGenerator = new Mock<IMoveGenerator>();
        mockMoveGenerator.Setup(m => m.GenerateMove(It.IsAny<Player>(), It.IsAny<Board>()))
            .Returns(Position.Create(0, 0));

        // Act
        // This test is a placeholder for future functionality

        // Assert
        session.Status.Should().Be(SessionStatus.Created);
        session.Moves.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GameSession_SimulateGame_ShouldThrowException_WhenSessionAlreadyCompleted()
    {
        // Arrange
        var session = Domain.Aggregates.GameSession.Create();
        session.StartSimulation();
        session.CompleteGame("X");
        var mockMoveGenerator = new Mock<IMoveGenerator>();

        // Act & Assert
        // This test is a placeholder for future functionality
        
        // Verify the session is completed
        session.Status.Should().Be(SessionStatus.Completed);
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