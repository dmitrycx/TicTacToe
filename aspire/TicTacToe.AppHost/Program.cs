var builder = DistributedApplication.CreateBuilder(args);

// Use a command-line argument or an environment variable to switch modes.
var useContainers = builder.Configuration["USE_CONTAINERS"] == "true" || 
                     args.Contains("--use-containers");
var useDockerfiles = builder.Configuration["USE_DOCKERFILE"] == "true" || 
                    args.Contains("--use-dockerfiles");

// --- Resource Declarations ---
IResourceBuilder<IResourceWithEndpoints> gameEngine;
IResourceBuilder<IResourceWithEndpoints> gameSession;
IResourceBuilder<IResourceWithEndpoints> nextJsApp;

if (useContainers)
{
    // --- Container Mode: All services from containers (Production Simulation) ---
    Console.WriteLine("Running in Container Mode (All services as containers)...");

    gameEngine = builder.AddContainer("game-engine", "tictactoe-gameengine:local-test")
        .WithHttpEndpoint(targetPort: 8080, name: "http");

    gameSession = builder.AddContainer("game-session", "tictactoe-gamesession:local-test")
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 8081, name: "http");

    nextJsApp = builder.AddContainer("nextjs-ui", "tictactoe-webui:local-test")
        .WithReference(gameSession.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 3000, env: "PORT")
        .WithExternalHttpEndpoints(); // Only expose the UI externally
}
else if (useDockerfiles)
{
    // --- Dockerfile Mode: Run from Dockerfiles for all services (test Dockerfile changes) ---
    Console.WriteLine("Running in Dockerfile Mode (All services from Dockerfiles)...");
    gameEngine = builder.AddDockerfile("game-engine", "../../GameEngine.Dockerfile", "../../")
        .WithHttpEndpoint(targetPort: 8080, name: "http");
    
    gameSession = builder.AddDockerfile("game-session", "../../GameSession.Dockerfile", "../../")
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 8081, name: "http");
    
    nextJsApp = builder.AddDockerfile("nextjs-ui", "../../WebUI.Dockerfile", "../../")
        .WithReference(gameSession.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 3000, env: "PORT")
        .WithExternalHttpEndpoints();
}
else
{
    // --- Local Mode: Run from project source (hot reload/dev) ---
    Console.WriteLine("Running in Project Mode (hot reload/dev)...");

    gameEngine = builder.AddProject<Projects.TicTacToe_GameEngine>("game-engine");
        
    gameSession = builder.AddProject<Projects.TicTacToe_GameSession>("game-session")
        .WithReference(gameEngine.GetEndpoint("http"));
    
    nextJsApp = builder.AddNodeApp("nextjs-ui", "../../src/TicTacToe.WebUI/aspire-entry.js")
        .WithReference(gameSession.GetEndpoint("http"))
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints();
}

// Apply common settings to the resources regardless of how they were created.
// Note: Only expose the UI externally; backend services use internal networking
nextJsApp.WithExternalHttpEndpoints();

builder.Build().Run();
