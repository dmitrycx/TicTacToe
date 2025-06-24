using Xunit;
using FastEndpoints;
using FluentAssertions;
using System.Net;
using System.Text;
using TicTacToe.GameEngine.Tests.Fixtures;

namespace TicTacToe.GameEngine.Tests.Features.CreateGame;

public class CreateGameConfigurationTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public CreateGameConfigurationTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task POST_Games_ShouldHaveCorrectRoute()
    {
        var client = _fixture.CreateClient();
        // Act
        var response = await client.PostAsync("/games", 
            new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_Games_ShouldAcceptJsonContent()
    {
        var client = _fixture.CreateClient();
        // Act
        var response = await client.PostAsync("/games", 
            new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task POST_Games_ShouldReturnJsonResponse()
    {
        var client = _fixture.CreateClient();
        // Act
        var response = await client.PostAsync("/games", 
            new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task POST_Games_ShouldNotRequireAuthentication()
    {
        var client = _fixture.CreateClient();
        // Act
        var response = await client.PostAsync("/games", 
            new StringContent("{}", Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
} 