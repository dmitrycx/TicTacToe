var builder = DistributedApplication.CreateBuilder(args);

// Use a command-line argument or an environment variable to switch modes.
bool useContainers = builder.Configuration["USE_CONTAINERS"] == "true" || 
                     args.Contains("--use-containers");

if (useContainers)
{
    // --- MODE 2: Run from pre-built Docker images ---
    // This is for CI validation and testing the final artifact.
    // It assumes you have already run `docker build` to create these images.

    Console.WriteLine("Running in Container Mode...");

    var gameEngine = builder.AddContainer("game-engine", "tictactoe-gameengine:local-test")
                            .WithHttpEndpoint(targetPort: 8080, name: "http");

    var gameSession = builder.AddContainer("game-session", "tictactoe-gamesession:local-test")
                             .WithReference(gameEngine.GetEndpoint("http"))
                             .WithHttpEndpoint(targetPort: 8081, name: "http");
}
else
{
    // --- MODE 1: Run from project source (Default) ---
    // This is for fast, inner-loop development with hot reload and debugging.

    Console.WriteLine("Running in Project Mode...");

    var gameEngine = builder.AddProject("game-engine", "../../src/TicTacToe.GameEngine/TicTacToe.GameEngine.csproj");

    var gameSession = builder.AddProject("game-session", "../../src/TicTacToe.GameSession/TicTacToe.GameSession.csproj")
                             .WithReference(gameEngine);
}

// Any other resources (like databases or caches) can be added here, outside the if/else.
// builder.AddRedis("cache");

builder.Build().Run();
