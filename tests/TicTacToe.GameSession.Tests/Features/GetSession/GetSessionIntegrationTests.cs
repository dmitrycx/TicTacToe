using Xunit;
using System.Net;
using System.Text.Json;
using TicTacToe.GameSession.Endpoints;
using TicTacToe.GameSession.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Persistence;

namespace TicTacToe.GameSession.Tests.Features.GetSession;

[Trait("Category", "Integration")]
public class GetSessionIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task GetSession_HappyPath_ShouldReturnSessionData()
    {
        // Arrange
        var session = new TicTacToe.GameSession.Domain.Aggregates.GameSession(Guid.NewGuid());
        await fixture.GameSessionRepository.SaveAsync(session);
        
        // Act
        var response = await _client.GetAsync($"/sessions/{session.Id}");
        var content = await response.Content.ReadAsStringAsync();
        var sessionResponse = JsonSerializer.Deserialize<GetSessionResponse>(content, _jsonOptions);
        
        // Assert
        Assert.Equal(200, (int)response.StatusCode);
        Assert.NotNull(sessionResponse);
        Assert.Equal(session.Id, sessionResponse.SessionId);
        Assert.Equal(session.GameId, sessionResponse.GameId);
        Assert.Equal("Created", sessionResponse.Status);
        Assert.Equal(session.CreatedAt, sessionResponse.CreatedAt);
        Assert.Null(sessionResponse.StartedAt);
        Assert.Null(sessionResponse.CompletedAt);
        Assert.Empty(sessionResponse.Moves);
        Assert.Null(sessionResponse.Winner);
        Assert.Null(sessionResponse.Result);
    }
    
    [Fact]
    public async Task GetSession_NotFound_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/sessions/{nonExistentId}");
        
        // Assert
        Assert.Equal(404, (int)response.StatusCode);
    }
} 