using System.ComponentModel.DataAnnotations;
using TicTacToe.GameSession.Infrastructure.Configuration;

namespace TicTacToe.GameSession.UnitTests.Infrastructure.Configuration;

public class GameEngineSettingsTests
{
    [Fact]
    public void DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var settings = new GameEngineSettings();

        // Assert
        Assert.Equal("http://localhost:5000", settings.BaseUrl);
        Assert.Equal(30, settings.TimeoutSeconds);
        Assert.Equal(3, settings.MaxRetries);
    }

    [Fact]
    public void BaseUrl_WithValidUrl_ShouldBeValid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "https://localhost:5001"
        };

        // Act & Assert
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void BaseUrl_WithInvalidUrl_ShouldBeInvalid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "not-a-valid-url"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.BaseUrl)));
    }

    [Fact]
    public void BaseUrl_WithEmptyString_ShouldBeInvalid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = string.Empty
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.BaseUrl)));
    }

    [Fact]
    public void BaseUrl_WithNullValue_ShouldBeInvalid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = null!
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.BaseUrl)));
    }

    [Theory]
    [InlineData("http://localhost:5000")]
    [InlineData("https://api.example.com")]
    [InlineData("https://localhost:5001/api")]
    public void BaseUrl_WithValidUrls_ShouldBeValid(string url)
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = url
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("localhost:5000")]
    public void BaseUrl_WithInvalidUrls_ShouldBeInvalid(string url)
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = url
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.BaseUrl)));
    }

    [Fact]
    public void TimeoutSeconds_WithValidValue_ShouldBeValid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "https://localhost:5001",
            TimeoutSeconds = 30
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void TimeoutSeconds_WithInvalidValues_ShouldBeInvalid(int timeout)
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "https://localhost:5001",
            TimeoutSeconds = timeout
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.TimeoutSeconds)));
    }

    [Fact]
    public void MaxRetries_WithValidValue_ShouldBeValid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "https://localhost:5001",
            MaxRetries = 3
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void MaxRetries_WithInvalidValues_ShouldBeInvalid(int maxRetries)
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "https://localhost:5001",
            MaxRetries = maxRetries
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.MaxRetries)));
    }

    [Fact]
    public void CompleteSettings_WithValidValues_ShouldBeValid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "https://localhost:5001",
            TimeoutSeconds = 30,
            MaxRetries = 3
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void CompleteSettings_WithMultipleInvalidValues_ShouldBeInvalid()
    {
        // Arrange
        var settings = new GameEngineSettings
        {
            BaseUrl = "invalid-url",
            TimeoutSeconds = -5,
            MaxRetries = -2
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, new ValidationContext(settings), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Equal(3, validationResults.Count);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.BaseUrl)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.TimeoutSeconds)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(GameEngineSettings.MaxRetries)));
    }
} 