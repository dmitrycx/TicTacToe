using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;
using TicTacToe.Shared.Enums;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

[Trait("Category", "Integration")]
public class GetSessionIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSession_HappyPath_ShouldReturnSessionData()
    {
        // Arrange - Create a session first
        var createRequest = new { strategy = GameStrategy.Random };
        var createResponse = await _client.PostAsync("/sessions", JsonContent.Create(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<CreateSessionResponse>(createContent, _jsonOptions);

        // Act
        var getResponse = await _client.GetAsync($"/sessions/{sessionResponse!.SessionId}");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var getSessionResponse = JsonSerializer.Deserialize<GetSessionResponse>(getContent, _jsonOptions);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getSessionResponse.Should().NotBeNull();
        getSessionResponse!.SessionId.Should().Be(sessionResponse.SessionId);
        getSessionResponse.Strategy.Should().Be(GameStrategy.Random);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSession_NotFound_ShouldReturn404()
    {
        // Arrange
        var nonExistentSessionId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/sessions/{nonExistentSessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 