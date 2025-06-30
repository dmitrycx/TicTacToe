# Setup Guide

Complete setup instructions for getting the TicTacToe project up and running.

## 🎯 **Quick Start (5 Minutes)**

### **Prerequisites**
- **.NET SDK 9.0.301** (see `global.json`)
- **Docker Desktop** (for containerized development)
- **Node.js 18+** (for Next.js frontend)
- **Git** (for version control)

### **1. Clone and Navigate**
```bash
git clone https://github.com/dmitrycx/TicTacToe.git
cd TicTacToe
```

### **2. Start with .NET Aspire (Recommended)**
```bash
# Start all services (backend + frontend) with hot reload
dotnet run --project aspire/TicTacToe.AppHost
```

### **3. Access the Application**
- **Next.js UI**: Available at the URL shown in Aspire dashboard
- **Aspire Dashboard**: https://localhost:17122
- **GameSession API**: Injected automatically via Aspire
- **GameEngine API**: Injected automatically via Aspire

## 🏗️ **Development Modes**

### **Default Mode (Recommended for Development)**
```bash
dotnet run --project aspire/TicTacToe.AppHost
```

**Features:**
- ⚡ **Instant startup** (in-memory persistence)
- 🔄 **Fast feedback** loops with hot reload
- 🚫 **No internet** required
- 💰 **No costs** incurred
- Perfect for daily development and feature work

### **Container Mode (Production Simulation)**
```bash
# Build containers first
docker build -f GameEngine.Dockerfile -t tictactoe-gameengine:local-test .
docker build -f GameSession.Dockerfile -t tictactoe-gamesession:local-test .
docker build -f ApiGateway.Dockerfile -t tictactoe-apigateway:local-test .
docker build -f WebUI.Dockerfile -t tictactoe-webui:local-test .

# Start with containers
dotnet run --project aspire/TicTacToe.AppHost -- --use-containers
```

**Features:**
- Full production simulation
- All services run as containers
- Uses Next.js API proxy routes for backend communication
- Useful for final testing before deployment

## 🧪 **Testing Setup**

### **Backend Testing**
```bash
# Unit tests (fast, in-memory)
dotnet test --filter "Category=Unit"

# Integration tests (in-memory)
dotnet test --filter "Category=Integration"

# All tests
dotnet test
```

### **Frontend Testing**
```bash
cd src/TicTacToe.WebUI

# Install dependencies
npm install

# Unit tests
npm test

# E2E tests
npm run test:e2e
```

### **Container Testing**
```bash
# Full container integration tests
./scripts/testing/test-containers-local.sh
```

## 🔍 **Verification**

### **Check Service Health**
```bash
# GameEngine health
curl http://localhost:8080/health

# GameSession health
curl http://localhost:8081/health

# API Gateway health
curl http://localhost:8082/health
```

### **Test API Endpoints**
```bash
# Create a game session
curl -X POST http://localhost:8081/sessions \
  -H "Content-Type: application/json" \
  -d '{"strategy": "Random"}'

# List sessions
curl http://localhost:8081/sessions
```

### **Check Frontend**
- Open the Next.js UI URL in your browser
- Verify the game board loads
- Test creating a new session
- Verify real-time updates work

## 🚨 **Troubleshooting**

### **Common Issues**

**Service Won't Start:**
```bash
# Check .NET version
dotnet --version  # Should be 9.0.301

# Clean and rebuild
dotnet clean
dotnet build
```

**Port Conflicts:**
```bash
# Check what's using the ports
lsof -i :8080  # GameEngine
lsof -i :8081  # GameSession
lsof -i :8082  # ApiGateway
lsof -i :3000  # Next.js UI
```

**Frontend Issues:**
```bash
cd src/TicTacToe.WebUI
npm install
npm run dev
```

## 📚 **Next Steps**

- **Development**: See [Developer Guide](../development/README.md)
- **Testing**: See [Testing Guide](../testing/README.md)
- **Deployment**: See [Deployment Guide](../deployment/README.md) 