#!/bin/bash
set -e # Exit immediately if a command exits with a non-zero status.

echo "--- Building tictactoe-gameengine:local-test ---"
docker build -f GameEngine.Dockerfile -t tictactoe-gameengine:local-test .

echo "--- Building tictactoe-gamesession:local-test ---"
docker build -f GameSession.Dockerfile -t tictactoe-gamesession:local-test .

echo "--- Building tictactoe-webui:local-test ---"
docker build -f WebUI.Dockerfile -t tictactoe-webui:local-test .

echo "--- All local container images built successfully! ---" 