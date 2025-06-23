namespace TicTacToe.GameSession.Domain.Constants;

/// <summary>
/// Constants for session-related strings and error messages.
/// </summary>
public static class SessionConstants
{
    /// <summary>
    /// Session status string constants.
    /// </summary>
    public static class Status
    {
        public const string Created = "Created";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }

    /// <summary>
    /// Error message constants.
    /// </summary>
    public static class ErrorMessages
    {
        public const string SessionNotInCreatedState = "Session is not in Created state";
        public const string SessionAlreadyCompleted = "Session is already completed";
        public const string CannotStartSimulation = "Cannot start simulation for session in {0} state";
        public const string CannotRecordMoves = "Cannot record moves to a session in the {0} state. Session must be InProgress";
        public const string CannotAddMoves = "Cannot add moves to a session in the {0} state. Session must be InProgress";
        public const string GameIdAlreadySet = "Game ID is already set";
        public const string MoveDoesNotBelongToSession = "Move does not belong to this session";
        public const string SimulationFailed = "Simulation failed";
    }
} 