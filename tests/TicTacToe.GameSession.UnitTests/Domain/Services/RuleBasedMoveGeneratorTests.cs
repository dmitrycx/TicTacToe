using TicTacToe.GameSession.Domain.Services;

namespace TicTacToe.GameSession.UnitTests.Domain.Services;

public class RuleBasedMoveGeneratorTests
{
    private readonly RuleBasedMoveGenerator _generator = new();

    [Fact]
    public void Type_ShouldReturnRuleBased()
    {
        // Act
        var moveType = _generator.Type;

        // Assert
        Assert.Equal(MoveType.RuleBased, moveType);
    }
} 