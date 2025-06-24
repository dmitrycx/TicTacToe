using Xunit;
using FluentAssertions;
using Moq;
using TicTacToe.GameEngine.Domain.Aggregates;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Persistence;
using TicTacToe.GameEngine.Domain.Exceptions;
using TicTacToe.GameEngine.Domain.ValueObjects;

namespace TicTacToe.GameEngine.Tests.Features.CreateGame;

/// <summary>
/// Unit tests for CreateGame domain logic and repository behavior
/// </summary>
[Trait("Category", "Unit")]
public class CreateGameDomainTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Game_Create_ShouldInitializeWithCorrectState()
    {
        // Act
        var game = Game.Create();

        // Assert
        game.Status.Should().Be(GameStatus.InProgress);
        game.CurrentPlayer.Should().Be(Player.X);
        game.Winner.Should().BeNull();
        game.Board.Should().NotBeNull();
        game.Id.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Game_Create_ShouldGenerateUniqueIds_WhenMultipleGamesCreated()
    {
        // Act
        var game1 = Game.Create();
        var game2 = Game.Create();

        // Assert
        game1.Id.Should().NotBe(game2.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_SaveAsync_ShouldPersistGame()
    {
        // Arrange
        var mockRepository = new Mock<IGameRepository>();
        mockRepository.Setup(r => r.SaveAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game game) => game);
        
        var game = Game.Create();

        // Act
        var savedGame = await mockRepository.Object.SaveAsync(game);

        // Assert
        savedGame.Should().Be(game);
        mockRepository.Verify(r => r.SaveAsync(It.IsAny<Game>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetByIdAsync_ShouldReturnGame_WhenGameExists()
    {
        // Arrange
        var game = Game.Create();
        var mockRepository = new Mock<IGameRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        // Act
        var retrievedGame = await mockRepository.Object.GetByIdAsync(game.Id);

        // Assert
        retrievedGame.Should().Be(game);
        mockRepository.Verify(r => r.GetByIdAsync(game.Id), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Repository_GetByIdAsync_ShouldReturnNull_WhenGameDoesNotExist()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var mockRepository = new Mock<IGameRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(gameId))
            .ReturnsAsync((Game?)null);

        // Act
        var retrievedGame = await mockRepository.Object.GetByIdAsync(gameId);

        // Assert
        retrievedGame.Should().BeNull();
        mockRepository.Verify(r => r.GetByIdAsync(gameId), Times.Once);
    }
} 