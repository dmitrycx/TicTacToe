namespace TicTacToe.GameSession.Endpoints;

/// <summary>
/// Static mapper for converting domain objects to DTOs.
/// </summary>
public static class SessionMapper
{
    /// <summary>
    /// Maps a GameSession domain object to GetSessionResponse DTO.
    /// </summary>
    /// <param name="session">The session to map.</param>
    /// <returns>The mapped response DTO.</returns>
    public static GetSessionResponse ToResponse(this TicTacToe.GameSession.Domain.Aggregates.GameSession session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        return new GetSessionResponse(
            session.Id,
            session.CurrentGameId,
            session.GameIds?.ToList() ?? new List<Guid>(),
            session.Status.ToString(),
            session.CreatedAt,
            session.StartedAt,
            session.CompletedAt,
            session.Moves?.Select(m => new MoveInfo(m.Position.Row, m.Position.Column, m.Player.ToString())).ToList() ?? new List<MoveInfo>(),
            session.Winner?.ToString(),
            session.Result?.ToString()
        );
    }

    /// <summary>
    /// Maps a GameSession domain object to SessionSummary DTO.
    /// </summary>
    /// <param name="session">The session to map.</param>
    /// <returns>The mapped summary DTO.</returns>
    public static SessionSummary ToSummary(this TicTacToe.GameSession.Domain.Aggregates.GameSession session)
    {
        return new SessionSummary(
            session.Id,
            session.Status.ToString(),
            session.CreatedAt,
            session.Moves.Count,
            session.Winner
        );
    }

    /// <summary>
    /// Maps a collection of GameSession domain objects to ListSessionsResponse DTO.
    /// </summary>
    /// <param name="sessions">The sessions to map.</param>
    /// <returns>The mapped response DTO.</returns>
    public static ListSessionsResponse ToResponse(this IEnumerable<TicTacToe.GameSession.Domain.Aggregates.GameSession> sessions)
    {
        var sessionSummaries = sessions.Select(s => s.ToSummary()).ToList();
        return new ListSessionsResponse(sessionSummaries);
    }
} 