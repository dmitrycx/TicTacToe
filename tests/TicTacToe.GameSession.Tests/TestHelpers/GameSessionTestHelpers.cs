using TicTacToe.GameSession.Domain.Aggregates;
using TicTacToe.GameSession.Domain.Entities;
using TicTacToe.GameSession.Domain.Enums;
using TicTacToe.GameSession.Domain.Events;
using TicTacToe.GameEngine.Domain.Enums;
using TicTacToe.GameEngine.Domain.ValueObjects;

namespace TicTacToe.GameSession.Tests.TestHelpers;

public static class GameSessionTestHelpers
{
    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateNewSession()
    {
        return TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();
    }

    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateSessionWithMoves(params (Player Player, int Row, int Col)[] moves)
    {
        var session = TicTacToe.GameSession.Domain.Aggregates.GameSession.Create();
        session.StartSimulation(); // Need to start simulation to add moves
        
        foreach (var (player, row, col) in moves)
        {
            var position = Position.Create(row, col);
            session.RecordMove(position, player);
        }
        
        return session;
    }

    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateCompletedSession(Player winner)
    {
        var session = CreateSessionWithMoves(
            (Player.X, 0, 0), (Player.O, 1, 0), 
            (Player.X, 0, 1), (Player.O, 1, 1), 
            (Player.X, 0, 2)
        );
        
        session.CompleteGame(winner.ToString());
        return session;
    }

    public static TicTacToe.GameSession.Domain.Aggregates.GameSession CreateDrawSession()
    {
        var session = CreateSessionWithMoves(
            (Player.X, 0, 0), (Player.O, 0, 1), (Player.X, 0, 2),
            (Player.O, 1, 0), (Player.X, 1, 1), (Player.O, 1, 2),
            (Player.X, 2, 0), (Player.O, 2, 1), (Player.X, 2, 2)
        );
        
        session.CompleteGame(null); // Draw
        return session;
    }
} 