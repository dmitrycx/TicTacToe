using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

[Trait("Category", "Integration")]
public class GetSessionIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSession_HappyPath_ShouldReturnSessionData()
    {
        // Arrange
        var session = new Domain.Aggregates.GameSession(Guid.NewGuid());
        await fixture.GameSessionRepository.SaveAsync(session);
        
        // Act
        var response = await _client.GetAsync($"/sessions/{session.Id}");
        var content = await response.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<GetSessionResponse>(content, _jsonOptions);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sessionResponse.Should().NotBeNull();
        sessionResponse!.SessionId.Should().Be(session.Id);
        sessionResponse.GameId.Should().Be(session.GameId);
        sessionResponse.Status.Should().Be("Created");
        sessionResponse.CreatedAt.Should().Be(session.CreatedAt);
        sessionResponse.StartedAt.Should().BeNull();
        sessionResponse.CompletedAt.Should().BeNull();
        sessionResponse.Moves.Should().BeEmpty();
        sessionResponse.Winner.Should().BeNull();
        sessionResponse.Result.Should().BeNull();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSession_NotFound_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/sessions/{nonExistentId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
} 