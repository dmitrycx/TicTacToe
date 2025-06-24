using Xunit;
using FluentAssertions;
using System.Text;
using TicTacToe.GameEngine.Tests.Fixtures;

namespace TicTacToe.GameEngine.Tests.Features.CreateGame;

[Trait("Category", "Unit")]
public class CreateGameConfigurationTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public CreateGameConfigurationTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task POST_Games_ShouldHaveCorrectRoute()
    {
        // Arrange
        var app = _fixture.CreateClient();

        // Act
        var response = await app.PostAsync("/games", new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert
        response.Should().NotBeNull();
        // The route should be accessible (even if it returns an error for empty content)
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task POST_Games_ShouldAcceptJsonContent()
    {
        // Arrange
        var app = _fixture.CreateClient();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = await app.PostAsync("/games", jsonContent);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task POST_Games_ShouldReturnJsonResponse()
    {
        // Arrange
        var app = _fixture.CreateClient();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = await app.PostAsync("/games", jsonContent);

        // Assert
        response.Should().NotBeNull();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task POST_Games_ShouldNotRequireAuthentication()
    {
        // Arrange
        var app = _fixture.CreateClient();
        var jsonContent = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = await app.PostAsync("/games", jsonContent);

        // Assert
        response.Should().NotBeNull();
        // Should not return 401 Unauthorized
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Unauthorized);
    }
} 