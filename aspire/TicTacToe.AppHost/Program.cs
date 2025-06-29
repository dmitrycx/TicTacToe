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
    // --- Container Mode ---
    gameEngine = builder.AddContainer("game-engine", "tictactoe-gameengine:local-test")
        .WithHttpEndpoint(targetPort: 8080, name: "http")
        .WithExternalHttpEndpoints();

    gameSession = builder.AddContainer("game-session", "tictactoe-gamesession:local-test")
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 8081, name: "http")
        .WithExternalHttpEndpoints();

    nextJsApp = builder.AddContainer("nextjs-ui", "tictactoe-webui:local-test")
        .WithReference(gameSession.GetEndpoint("http")) // ONLY the reference is needed
        .WithHttpEndpoint(targetPort: 3000, env: "PORT")
        .WithExternalHttpEndpoints();
}
else if (useDockerfiles)
{
    // --- Dockerfile Mode ---
    var appHostDirectory = builder.Environment.ContentRootPath;
    var solutionRoot = Path.GetFullPath(Path.Combine(appHostDirectory, "..", ".."));

    gameEngine = builder.AddDockerfile("game-engine", Path.Combine(solutionRoot, "GameEngine.Dockerfile"), solutionRoot)
        .WithHttpEndpoint(targetPort: 8080, name: "http")
        .WithExternalHttpEndpoints();

    gameSession = builder.AddDockerfile("game-session", Path.Combine(solutionRoot, "GameSession.Dockerfile"), solutionRoot)
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 8081, name: "http")
        .WithExternalHttpEndpoints();

    nextJsApp = builder.AddDockerfile("nextjs-ui", Path.Combine(solutionRoot, "WebUI.Dockerfile"), solutionRoot)
        .WithReference(gameSession.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 3000, env: "PORT")
        .WithExternalHttpEndpoints();
}
else
{
    // --- Local Mode ---
    gameEngine = builder.AddProject<Projects.TicTacToe_GameEngine>("game-engine")
        .WithExternalHttpEndpoints();

    gameSession = builder.AddProject<Projects.TicTacToe_GameSession>("game-session")
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithExternalHttpEndpoints();

    nextJsApp = builder.AddNodeApp("nextjs-ui", "../../src/TicTacToe.WebUI/aspire-entry.js")
        .WithReference(gameSession.GetEndpoint("http"))
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints();
}

builder.Build().Run();