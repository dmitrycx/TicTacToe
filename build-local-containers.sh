#!/bin/bash
set -e # Exit immediately if a command exits with a non-zero status.

# Build all service images for local testing
echo "Building TicTacToe containers for local testing..."

# Build GameEngine
echo "Building GameEngine container..."
docker build -f GameEngine.Dockerfile -t tictactoe-gameengine:local-test .

# Build GameSession
echo "Building GameSession container..."
docker build -f GameSession.Dockerfile -t tictactoe-gamesession:local-test .

# Build API Gateway
echo "Building API Gateway container..."
docker build -f ApiGateway.Dockerfile -t tictactoe-apigateway:local-test .

# Build WebUI
echo "Building WebUI container..."
docker build -f WebUI.Dockerfile -t tictactoe-webui:local-test .

echo "All containers built successfully!"
echo ""
echo "Available images:"
echo "  - tictactoe-gameengine:local-test"
echo "  - tictactoe-gamesession:local-test"
echo "  - tictactoe-apigateway:local-test"
echo "  - tictactoe-webui:local-test"
echo ""
echo "To run with Aspire:"
echo "  dotnet run --project aspire/TicTacToe.AppHost -- --use-containers" 