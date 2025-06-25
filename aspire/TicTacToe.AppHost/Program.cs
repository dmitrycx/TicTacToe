var builder = DistributedApplication.CreateBuilder(args);

// Add GameEngine service. Aspire will assign a random port to the default 'http' endpoint.
var gameEngine = builder.AddProject("game-engine", "../../src/TicTacToe.GameEngine/TicTacToe.GameEngine.csproj");

// Add GameSession service.
var gameSession = builder.AddProject("game-session", "../../src/TicTacToe.GameSession/TicTacToe.GameSession.csproj")
                         .WithReference(gameEngine); // Pass the reference to enable service discovery.

builder.Build().Run();
