using Xunit;
using TicTacToe.GameEngine.Domain.ValueObjects;

namespace TicTacToe.GameEngine.UnitTests.Domain.ValueObjects;

public class PositionTests
{
    [Fact]
    public void Create_WithValidCoordinates_ShouldSucceed()
    {
        // Arrange & Act
        var position = Position.Create(1, 2);

        // Assert
        Assert.NotNull(position);
        Assert.Equal(1, position.Row);
        Assert.Equal(2, position.Column);
    }

    [Theory]
    [InlineData(-1, 0)]  // Row too low
    [InlineData(3, 0)]   // Row too high
    [InlineData(0, -1)]  // Column too low
    [InlineData(0, 3)]   // Column too high
    [InlineData(5, 5)]   // Both out of bounds
    public void Create_WithInvalidCoordinates_ShouldThrowArgumentException(int row, int column)
    {
        // Arrange
        var action = () => Position.Create(row, column);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Position must be within the 3x3 board bounds (0-2).", exception.Message);
    }

    [Fact]
    public void TwoPositionsWithSameCoordinates_ShouldBeEqual()
    {
        // Arrange
        var position1 = Position.Create(2, 1);
        var position2 = Position.Create(2, 1);

        // Act & Assert
        Assert.Equal(position1, position2);
        Assert.True(position1 == position2);
    }
}