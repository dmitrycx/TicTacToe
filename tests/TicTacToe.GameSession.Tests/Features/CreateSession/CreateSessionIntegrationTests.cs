using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;
using TicTacToe.Shared.Enums;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace TicTacToe.GameSession.Tests.Features.CreateSession;

[Trait("Category", "Integration")]
public class CreateSessionIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    [Trait("Category", "Integration")]
    public async Task POST_Sessions_ShouldCreateNewSession_WhenValidRequest()
    {
        // Arrange
        var request = new { strategy = GameStrategy.Random };

        // Act
        var response = await _client.PostAsync("/sessions", JsonContent.Create(request));
        var responseContent = await response.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<CreateSessionResponse>(responseContent, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        sessionResponse.Should().NotBeNull();
        sessionResponse!.SessionId.Should().NotBeEmpty();
        sessionResponse.Strategy.Should().Be(GameStrategy.Random);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task POST_Sessions_ShouldReturnUniqueSessionIds_WhenMultipleRequests()
    {
        // Arrange
        var request = new { strategy = GameStrategy.Random };

        // Act
        var response1 = await _client.PostAsync("/sessions", JsonContent.Create(request));
        var response2 = await _client.PostAsync("/sessions", JsonContent.Create(request));

        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();

        var session1 = JsonSerializer.Deserialize<CreateSessionResponse>(content1, _jsonOptions);
        var session2 = JsonSerializer.Deserialize<CreateSessionResponse>(content2, _jsonOptions);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        session1!.SessionId.Should().NotBe(session2!.SessionId);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task POST_Sessions_ShouldPersistSessionInRepository_WhenSuccessful()
    {
        // Arrange
        var request = new { strategy = GameStrategy.RuleBased };

        // Act
        var response = await _client.PostAsync("/sessions", JsonContent.Create(request));
        var responseContent = await response.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<CreateSessionResponse>(responseContent, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        sessionResponse.Should().NotBeNull();
        
        // Verify session exists in repository
        var repository = fixture.GameSessionRepository;
        var session = await repository.GetByIdAsync(sessionResponse!.SessionId);
        session.Should().NotBeNull();
        session!.Strategy.Should().Be(GameStrategy.RuleBased);
    }
} 