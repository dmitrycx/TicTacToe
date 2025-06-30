#!/bin/bash
set -e # Exit immediately if a command exits with a non-zero status.

echo "🐳 Running Container Integration Tests Locally"
echo "=============================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker Desktop first."
    exit 1
fi

# Check if containers exist
print_status "Checking if container images exist..."

if ! docker image inspect tictactoe-gameengine:local-test > /dev/null 2>&1; then
    print_warning "GameEngine container not found. Building..."
    docker build -f GameEngine.Dockerfile -t tictactoe-gameengine:local-test .
fi

if ! docker image inspect tictactoe-gamesession:local-test > /dev/null 2>&1; then
    print_warning "GameSession container not found. Building..."
    docker build -f GameSession.Dockerfile -t tictactoe-gamesession:local-test .
fi

# Create docker-compose file for testing
print_status "Creating test docker-compose file..."

cat <<EOF > docker-compose.test.yml
version: '3.8'
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

# Start services
print_status "Starting services..."
docker compose -f docker-compose.test.yml up -d

# Wait for services to be healthy
print_status "Waiting for services to become healthy..."
timeout 60 bash -c 'until docker compose -f docker-compose.test.yml ps | grep -q "healthy"; do sleep 5; echo "Waiting for services..."; done' || {
    print_warning "Services may not be fully healthy, proceeding with tests"
}

# Run the container integration tests
print_status "Running container integration tests..."
dotnet test ./tests/TicTacToe.GameSession.Tests/TicTacToe.GameSession.Tests.csproj \
    --filter "Category=ContainerIntegration" \
    --logger "console;verbosity=detailed" \
    --verbosity normal

# Capture test result
TEST_RESULT=$?

# Stop services
print_status "Stopping services..."
docker compose -f docker-compose.test.yml down

# Clean up
rm -f docker-compose.test.yml

# Report result
if [ $TEST_RESULT -eq 0 ]; then
    print_status "✅ All container integration tests passed!"
    exit 0
else
    print_error "❌ Some container integration tests failed!"
    exit 1
fi 