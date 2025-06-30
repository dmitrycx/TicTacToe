#!/bin/bash

# CI-Like Container Testing Script
# This script mimics GitHub Actions conditions to catch CI failures locally

set -e  # Exit on any error

echo "🐳 Running Container Integration Tests (CI-like conditions)..."
echo "=================================================="

# Check if container images exist
echo "[INFO] Checking if container images exist..."
if ! docker image inspect tictactoe-gameengine:local-test >/dev/null 2>&1; then
    echo "[ERROR] Container image tictactoe-gameengine:local-test not found!"
    echo "[INFO] Run ./build-local-containers.sh first"
    exit 1
fi

if ! docker image inspect tictactoe-gamesession:local-test >/dev/null 2>&1; then
    echo "[ERROR] Container image tictactoe-gamesession:local-test not found!"
    echo "[INFO] Run ./build-local-containers.sh first"
    exit 1
fi

echo "[INFO] ✅ Container images found"

# Create test docker-compose file (mimics CI exactly)
echo "[INFO] Creating CI-like test docker-compose file..."
cat <<EOF > docker-compose.ci-local.yml
services:
  game-engine:
    image: tictactoe-gameengine:local-test
    hostname: game-engine
    ports:
      - "8080:8080"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/alive"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s
  game-session:
    image: tictactoe-gamesession:local-test
    ports:
      - "8081:8081"
    environment:
      - GameEngine__BaseUrl=http://game-engine:8080
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8081/alive"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s
    depends_on:
      game-engine:
        condition: service_healthy
EOF

echo "[INFO] Starting services with CI-like configuration..."

# Start services
docker compose -f docker-compose.ci-local.yml up -d

# Wait for services to be healthy (more aggressive than local script)
echo "[INFO] Waiting for services to become healthy..."
echo "[INFO] This may take up to 60 seconds..."

# More robust health check waiting
max_attempts=30
attempt=0
while [ $attempt -lt $max_attempts ]; do
    if docker compose -f docker-compose.ci-local.yml ps | grep -q "healthy"; then
        echo "[INFO] ✅ Services are healthy!"
        break
    fi
    
    attempt=$((attempt + 1))
    echo "[INFO] Waiting for services to become healthy... (attempt $attempt/$max_attempts)"
    sleep 2
done

if [ $attempt -eq $max_attempts ]; then
    echo "[WARNING] Services may not be fully healthy, but proceeding with tests"
fi

# Additional wait to ensure services are fully ready (mimics CI timing)
echo "[INFO] Additional wait to ensure services are fully ready..."
sleep 10

# Test service connectivity before running tests
echo "[INFO] Testing service connectivity..."
if ! curl -f http://localhost:8080/alive >/dev/null 2>&1; then
    echo "[ERROR] GameEngine service is not responding on port 8080"
    docker compose -f docker-compose.ci-local.yml logs game-engine
    exit 1
fi

if ! curl -f http://localhost:8081/alive >/dev/null 2>&1; then
    echo "[ERROR] GameSession service is not responding on port 8081"
    docker compose -f docker-compose.ci-local.yml logs game-session
    exit 1
fi

echo "[INFO] ✅ Service connectivity confirmed"

# Run tests with CI-like conditions
echo "[INFO] Running container integration tests..."
dotnet restore ./tests/TicTacToe.GameSession.Tests/TicTacToe.GameSession.Tests.csproj
dotnet build ./tests/TicTacToe.GameSession.Tests/TicTacToe.GameSession.Tests.csproj --no-restore
dotnet test ./tests/TicTacToe.GameSession.Tests/TicTacToe.GameSession.Tests.csproj --no-build --filter "Category=ContainerIntegration" --logger "console;verbosity=detailed"

# Capture test results
test_exit_code=$?

# Cleanup
echo "[INFO] Stopping services..."
docker compose -f docker-compose.ci-local.yml down

# Clean up the temporary compose file
rm -f docker-compose.ci-local.yml

# Report results
if [ $test_exit_code -eq 0 ]; then
    echo "[INFO] ✅ All container integration tests passed in CI-like conditions!"
else
    echo "[ERROR] ❌ Some container integration tests failed in CI-like conditions!"
    echo "[INFO] This indicates potential CI failures. Check the test output above."
fi

exit $test_exit_code 