namespace TicTacToe.GameSession.UnitTests.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void InvalidSessionStateException_ShouldHaveCorrectMessage()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var exception = new InvalidSessionStateException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void InvalidSessionStateException_ShouldInheritFromException()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var exception = new InvalidSessionStateException(message);

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void SessionNotFoundException_ShouldHaveCorrectMessage()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var expectedMessage = $"Session with ID {sessionId} was not found.";

        // Act
        var exception = new SessionNotFoundException(sessionId);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void SessionNotFoundException_ShouldHaveCorrectSessionId()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var exception = new SessionNotFoundException(sessionId);

        // Assert
        Assert.Equal(sessionId, exception.SessionId);
    }

    [Fact]
    public void SessionNotFoundException_ShouldInheritFromException()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        var exception = new SessionNotFoundException(sessionId);

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void InvalidSessionStateException_WithInnerException_ShouldPreserveInnerException()
    {
        // Arrange
        var message = "Test error message";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new InvalidSessionStateException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void SessionNotFoundException_WithInnerException_ShouldPreserveInnerException()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new SessionNotFoundException(sessionId, innerException);

        // Assert
        Assert.Equal(sessionId, exception.SessionId);
        Assert.Equal(innerException, exception.InnerException);
    }
} 