using Xunit;
using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToe.GameEngine.Endpoints;
using TicTacToe.GameEngine.Tests.Fixtures;

namespace TicTacToe.GameEngine.Tests.Features.CreateGame;

[Trait("Category", "Integration")]
public class CreateGameIntegrationTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public CreateGameIntegrationTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task POST_Games_ShouldCreateNewGame_WhenValidRequest()
    {
        // Arrange
        var client = _fixture.CreateClient();
        var request = new { };

        // Act
        var response = await client.PostAsync("/games", 
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateGameResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.Should().NotBeNull();
        result!.GameId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task POST_Games_ShouldReturnUniqueGameIds_WhenMultipleRequests()
    {
        // Arrange
        var client = _fixture.CreateClient();
        var request = new { };

        // Act
        var response1 = await client.PostAsync("/games", 
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        var response2 = await client.PostAsync("/games", 
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();
        
        var result1 = JsonSerializer.Deserialize<CreateGameResponse>(content1, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        var result2 = JsonSerializer.Deserialize<CreateGameResponse>(content2, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result1!.GameId.Should().NotBe(result2!.GameId);
    }
} 