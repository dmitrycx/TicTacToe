using TicTacToe.GameSession.UnitTests.TestHelpers;

namespace TicTacToe.GameSession.UnitTests.Domain.Aggregates;

public class GameSessionTests
{
    [Fact]
    public void Create_ShouldCreateSessionWithCorrectInitialState()
    {
        // Act
        var session = GameSessionFactory.CreateCreatedSession();

        // Assert
        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(SessionStatus.Created, session.Status);
        Assert.Equal(Guid.Empty, session.GameId);
        Assert.Empty(session.Moves);
        Assert.Null(session.Result);
        Assert.Null(session.Winner);
        Assert.NotNull(session.CreatedAt);
        Assert.Null(session.StartedAt);
        Assert.Null(session.CompletedAt);
    }

    [Fact]
    public void Create_WithGameId_ShouldCreateSessionWithSpecifiedGameId()
    {
        // Arrange
        var gameId = Guid.NewGuid();

        // Act
        var session = GameSessionFactory.CreateCreatedSession(gameId);

        // Assert
        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(gameId, session.GameId);
        Assert.Equal(SessionStatus.Created, session.Status);
    }

    [Fact]
    public void StartSimulation_WhenStatusIsCreated_ShouldChangeStatusToInProgress()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();

        // Act
        session.StartSimulation();

        // Assert
        Assert.Equal(SessionStatus.InProgress, session.Status);
        Assert.NotNull(session.StartedAt);
    }

    [Fact]
    public void StartSimulation_WhenStatusIsNotCreated_ShouldThrowException()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();

        // Act & Assert
        var exception = Assert.Throws<InvalidSessionStateException>(() => session.StartSimulation());
        Assert.Contains("Cannot start simulation", exception.Message);
    }

    [Fact]
    public void RecordMove_WhenStatusIsInProgress_ShouldAddMove()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        var position = new Position(0, 0);
        var player = Player.X;

        // Act
        session.RecordMove(position, player);

        // Assert
        Assert.Single(session.Moves);
        var move = session.Moves.First();
        Assert.Equal(session.Id, move.SessionId);
        Assert.Equal(position, move.Position);
        Assert.Equal(player, move.Player);
    }

    [Fact]
    public void RecordMove_WhenStatusIsNotInProgress_ShouldThrowException()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();
        var position = new Position(0, 0);
        var player = Player.X;

        // Act & Assert
        var exception = Assert.Throws<InvalidSessionStateException>(() => session.RecordMove(position, player));
        Assert.Contains("Cannot record moves", exception.Message);
    }

    [Fact]
    public void AddMove_WhenStatusIsInProgress_ShouldAddMove()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        var move = MoveFactory.CreateXMove(session.Id, 0, 0);

        // Act
        session.AddMove(move);

        // Assert
        Assert.Single(session.Moves);
        Assert.Contains(move, session.Moves);
    }

    [Fact]
    public void AddMove_WhenMoveBelongsToDifferentSession_ShouldThrowException()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        var differentSessionId = Guid.NewGuid();
        var move = MoveFactory.CreateXMove(differentSessionId, 0, 0);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => session.AddMove(move));
        Assert.Contains("does not belong to this session", exception.Message);
    }

    [Fact]
    public void Complete_WhenStatusIsNotCompleted_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        var result = GameStatus.Win;

        // Act
        session.Complete(result);

        // Assert
        Assert.Equal(SessionStatus.Completed, session.Status);
        Assert.Equal(result, session.Result);
        Assert.NotNull(session.CompletedAt);
    }

    [Fact]
    public void Complete_WhenStatusIsAlreadyCompleted_ShouldThrowException()
    {
        // Arrange
        var session = GameSessionFactory.CreateCompletedSession();

        // Act & Assert
        var exception = Assert.Throws<InvalidSessionStateException>(() => session.Complete(GameStatus.Win));
        Assert.Contains("already completed", exception.Message);
    }

    [Fact]
    public void CompleteGame_WithWinner_ShouldSetWinnerAndStatus()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        var winner = "X";

        // Act
        session.CompleteGame(winner);

        // Assert
        Assert.Equal(SessionStatus.Completed, session.Status);
        Assert.Equal(winner, session.Winner);
        Assert.Equal(GameStatus.Win, session.Result);
        Assert.NotNull(session.CompletedAt);
    }

    [Fact]
    public void CompleteGame_WithDraw_ShouldSetDrawStatus()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();

        // Act
        session.CompleteGame(null);

        // Assert
        Assert.Equal(SessionStatus.Completed, session.Status);
        Assert.Null(session.Winner);
        Assert.Equal(GameStatus.Draw, session.Result);
    }

    [Fact]
    public void FailSimulation_ShouldChangeStatusToFailed()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();

        // Act
        session.FailSimulation();

        // Assert
        Assert.Equal(SessionStatus.Failed, session.Status);
        Assert.NotNull(session.CompletedAt);
    }

    [Fact]
    public void SetGameId_WhenGameIdIsEmpty_ShouldSetGameId()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();
        var gameId = Guid.NewGuid();

        // Act
        session.SetGameId(gameId);

        // Assert
        Assert.Equal(gameId, session.GameId);
    }

    [Fact]
    public void SetGameId_WhenGameIdIsAlreadySet_ShouldThrowException()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var session = GameSessionFactory.CreateCreatedSession(gameId);

        // Act & Assert
        var exception = Assert.Throws<InvalidSessionStateException>(() => session.SetGameId(Guid.NewGuid()));
        Assert.Contains("already set", exception.Message);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        Assert.NotEmpty(session.DomainEvents);

        // Act
        session.ClearDomainEvents();

        // Assert
        Assert.Empty(session.DomainEvents);
    }

    [Fact]
    public void DomainEvents_ShouldContainSessionCreatedEvent_WhenSessionIsCreated()
    {
        // Act
        var session = GameSessionFactory.CreateCreatedSession();

        // Assert
        Assert.Single(session.DomainEvents);
        var domainEvent = session.DomainEvents.First();
        Assert.IsType<SessionCreatedEvent>(domainEvent);
        
        var sessionCreatedEvent = (SessionCreatedEvent)domainEvent;
        Assert.Equal(session.Id, sessionCreatedEvent.SessionId);
        Assert.Equal(session.GameId, sessionCreatedEvent.GameId);
    }

    [Fact]
    public void DomainEvents_ShouldContainSimulationStartedEvent_WhenSimulationStarts()
    {
        // Arrange
        var session = GameSessionFactory.CreateCreatedSession();
        session.ClearDomainEvents();

        // Act
        session.StartSimulation();

        // Assert
        Assert.Single(session.DomainEvents);
        var domainEvent = session.DomainEvents.First();
        Assert.IsType<SimulationStartedEvent>(domainEvent);
        
        var simulationStartedEvent = (SimulationStartedEvent)domainEvent;
        Assert.Equal(session.Id, simulationStartedEvent.SessionId);
    }

    [Fact]
    public void DomainEvents_ShouldContainMoveMadeEvent_WhenMoveIsRecorded()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        session.ClearDomainEvents();
        var position = new Position(0, 0);
        var player = Player.X;

        // Act
        session.RecordMove(position, player);

        // Assert
        Assert.Single(session.DomainEvents);
        var domainEvent = session.DomainEvents.First();
        Assert.IsType<MoveMadeEvent>(domainEvent);
        
        var moveMadeEvent = (MoveMadeEvent)domainEvent;
        Assert.Equal(session.Id, moveMadeEvent.SessionId);
        Assert.Equal(position, moveMadeEvent.Move.Position);
        Assert.Equal(player, moveMadeEvent.Move.Player);
    }

    [Fact]
    public void DomainEvents_ShouldContainGameCompletedEvent_WhenGameIsCompleted()
    {
        // Arrange
        var session = GameSessionFactory.CreateInProgressSession();
        session.ClearDomainEvents();
        var result = GameStatus.Win;

        // Act
        session.Complete(result);

        // Assert
        Assert.Single(session.DomainEvents);
        var domainEvent = session.DomainEvents.First();
        Assert.IsType<GameCompletedEvent>(domainEvent);
        
        var gameCompletedEvent = (GameCompletedEvent)domainEvent;
        Assert.Equal(session.Id, gameCompletedEvent.SessionId);
        Assert.Equal(result, gameCompletedEvent.Result);
    }
} 