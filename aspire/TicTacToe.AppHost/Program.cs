var builder = DistributedApplication.CreateBuilder(args);

// Use a command-line argument or an environment variable to switch modes.
var useContainers = builder.Configuration["USE_CONTAINERS"] == "true" ||
                     args.Contains("--use-containers");

// --- Resource Declarations ---
IResourceBuilder<IResourceWithEndpoints> gameEngine;
IResourceBuilder<IResourceWithEndpoints> gameSession;
IResourceBuilder<IResourceWithEndpoints> apiGateway;
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

    apiGateway = builder.AddContainer("api-gateway", "tictactoe-apigateway:local-test")
        .WithReference(gameSession.GetEndpoint("http"))
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 8082, name: "http")
        .WithExternalHttpEndpoints();

    nextJsApp = builder.AddContainer("nextjs-ui", "tictactoe-webui:local-test")
        .WithReference(apiGateway.GetEndpoint("http"))
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

    apiGateway = builder.AddProject<Projects.TicTacToe_ApiGateway>("api-gateway")
        .WithReference(gameSession.GetEndpoint("http"))
        .WithReference(gameEngine.GetEndpoint("http"))
        .WithExternalHttpEndpoints();

    nextJsApp = builder.AddNodeApp("nextjs-ui", "../../src/TicTacToe.WebUI/aspire-entry.js")
        .WithReference(apiGateway.GetEndpoint("http"))
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints();
}

builder.Build().Run();