var builder = DistributedApplication.CreateBuilder(args);

// Use a command-line argument or an environment variable to switch modes.
var useContainers = builder.Configuration["USE_CONTAINERS"] == "true" || 
                     args.Contains("--use-containers");
var useDockerfiles = builder.Configuration["USE_DOCKERFILE"] == "true" || 
                    args.Contains("--use-dockerfiles");

// --- Resource Declarations ---
IResourceBuilder<IResourceWithEndpoints> gameEngine;
IResourceBuilder<IResourceWithEndpoints> gameSession;

if (useContainers)
{
    // --- MODE 2: Run from pre-built Docker images ---
    // It assumes you have already run `docker build` to create these images.
    Console.WriteLine("Running in Container Mode...");
    
    gameEngine = builder.AddContainer("game-engine", "tictactoe-gameengine:local-test")
                            .WithHttpEndpoint(targetPort: 8080, name: "http");
    
    gameSession = builder.AddContainer("game-session", "tictactoe-gamesession:local-test")
                             .WithReference(gameEngine.GetEndpoint("http"))
                             .WithHttpEndpoint(targetPort: 8081, name: "http");
}
else if (useDockerfiles)
{
        
    // --- MODE 3: Run from Dockerfiles ---
    Console.WriteLine("Running in Dockerfile Mode...");
    gameEngine = builder.AddDockerfile("game-engine", "../../GameEngine.Dockerfile", "../../")
        .WithHttpEndpoint(targetPort: 8080, name: "http");
    
    gameSession = builder.AddDockerfile("game-session", "../../GameSession.Dockerfile", "../../")
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 8081, name: "http");
}
else
{
    // --- MODE 1: Run from project source (Default) ---
    // This is for fast, inner-loop development with hot reload and debugging.
    Console.WriteLine("Running in Project Mode...");

    gameEngine = builder.AddProject<Projects.TicTacToe_GameEngine>("game-engine");
        
    gameSession = builder.AddProject<Projects.TicTacToe_GameSession>("game-session")
        .WithReference(gameEngine.GetEndpoint("http"));
}

// Apply common settings to the resources regardless of how they were created.
gameEngine.WithExternalHttpEndpoints();
gameSession.WithExternalHttpEndpoints();

builder.Build().Run();
