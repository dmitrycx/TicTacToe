# Tic Tac Toe - Distributed Microservices Architecture

[![CI Pipeline](https://img.shields.io/github/actions/workflow/status/dmitrycx/TicTacToe/ci.yml?branch=main&style=for-the-badge)](https://github.com/dmitrycx/TicTacToe/actions)
[![.NET Version](https://img.shields.io/badge/.NET-9.0.200-blue?style=for-the-badge)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=for-the-badge)](LICENSE)

A production-ready demonstration of modern microservices architecture, featuring automated Tic Tac Toe gameplay with real-time visualization. Built with .NET 9, Next.js 15, and orchestrated by .NET Aspire.

## 🎯 Overview

This project showcases a distributed system where two AI players compete in Tic Tac Toe, with the entire game orchestrated by microservices and visualized in real-time through a modern web interface. The architecture demonstrates enterprise-grade patterns including:

- **Domain-Driven Design** with clean separation of concerns
- **Event-driven communication** via SignalR for real-time updates
- **Backend for Frontend (BFF)** pattern for secure API access
- **Container-first development** with comprehensive CI/CD
- **Comprehensive testing** strategy with unit and integration tests
- **Modern orchestration** with .NET Aspire for seamless development and deployment

## 🏗️ Architecture

The system employs a distributed microservices architecture with clear boundaries and responsibilities:

```mermaid
graph TB
    subgraph "Client Layer"
        UI[Next.js UI<br/>React + TypeScript]
    end

    subgraph "API Gateway / BFF"
        BFF[Next.js Server<br/>Backend for Frontend<br/>+ API Proxy Routes]
    end

    subgraph "Microservices Layer"
        GS[GameSession Service<br/>Session Orchestrator]
        GE[GameEngine Service<br/>Game Logic Engine]
    end

    subgraph "Communication"
        SR[SignalR Hub<br/>Real-time Updates]
    end

    UI -->|HTTP API Calls| BFF
    UI -->|WebSocket via Proxy| SR
    BFF -->|HTTP Proxy| GS
    BFF -->|HTTP Proxy| GE
    GS -->|HTTP| GE
    GS -->|Events| SR

    style UI fill:#61DAFB
    style BFF fill:#333,color:#fff
    style GS fill:#512BD4,color:#fff
    style GE fill:#512BD4,color:#fff
    style SR fill:#FF6B35
```

### Service Responsibilities

| Service | Technology | Port | Responsibility |
|---------|------------|------|----------------|
| **GameEngine** | .NET 9 + FastEndpoints | 8080 | Core game logic, board state, move validation |
| **GameSession** | .NET 9 + FastEndpoints + SignalR | 8081 | Session management, AI move orchestration |
| **WebUI** | Next.js 15 + React + TypeScript | 3000 | User interface with real-time updates |

## 🔄 System Interactions

### Game Simulation Flow

```mermaid
sequenceDiagram
    participant UI as UI
    participant GS as Game Session Service
    participant GE as Game Engine Service
    
    UI->>GS: POST /sessions
    GS->>GE: POST /games (create game)
    GE-->>GS: Game created
    GS-->>UI: Session created
    
    UI->>GS: POST /sessions/{id}/simulate
    loop Until game ends
        GS->>GS: Generate move for current player
        GS->>GE: POST /games/{id}/move
        GE-->>GS: Game state updated
        GS->>GS: Store move in history
    end
    GS-->>UI: Game completed
```

### Real-Time Communication Flow

```mermaid
sequenceDiagram
    participant UI as Next.js UI
    participant BFF as Next.js BFF
    participant GS as GameSession Service
    participant SR as SignalR Hub
    
    UI->>BFF: GET /api/game/sessions
    BFF->>GS: GET /sessions (proxied)
    GS-->>BFF: Sessions list
    BFF-->>UI: Sessions data
    
    UI->>BFF: Connect to /api/game/gameHub (proxied)
    BFF->>SR: WebSocket connection
    SR-->>BFF: Connection established
    BFF-->>UI: Connection established
    
    UI->>BFF: POST /api/game/sessions/{id}/simulate
    BFF->>GS: POST /sessions/{id}/simulate (proxied)
    GS->>GS: Start simulation
    
    loop During simulation
        GS->>SR: Broadcast move update
        SR->>BFF: Real-time move notification
        BFF->>UI: Move notification (proxied)
        UI->>UI: Update board display
    end
    
    GS->>SR: Broadcast game completion
    SR->>BFF: Game end notification
    BFF->>UI: Game end notification (proxied)
```

### Data Flow Architecture

```mermaid
graph LR
    subgraph "Frontend Layer"
        UI[React Components]
        State[React State]
    end
    
    subgraph "BFF Layer"
        API[API Routes]
        Cache[Response Cache]
    end
    
    subgraph "Service Layer"
        GS[GameSession]
        GE[GameEngine]
    end
    
    subgraph "Data Layer"
        SessionDB[(Session Store)]
        GameDB[(Game Store)]
    end
    
    UI --> State
    State --> API
    API --> Cache
    API --> GS
    GS --> GE
    GS --> SessionDB
    GE --> GameDB
    
    style UI fill:#61DAFB
    style API fill:#333,color:#fff
    style GS fill:#512BD4,color:#fff
    style GE fill:#512BD4,color:#fff
    style SessionDB fill:#28a745
    style GameDB fill:#28a745
```

## 🚀 Quick Start

### Prerequisites

- **.NET SDK 9.0.200** (see `global.json`)
- **Docker Desktop** (for containerized development)
- **Node.js 18+** (for Next.js frontend)
- **Git** (for version control)

### Development Setup

1. **Clone and Navigate**
   ```bash
   git clone https://github.com/dmitrycx/TicTacToe.git
   cd TicTacToe
   ```

2. **Start with .NET Aspire (Recommended)**
   ```bash
   # Start all services (backend + frontend) with hot reload
   dotnet run --project aspire/TicTacToe.AppHost
   ```
   
   This will start:
   - **GameEngine** service (.NET project with hot reload)
   - **GameSession** service (.NET project with hot reload)  
   - **Next.js UI** (dev server with hot reload)
   - **Aspire Dashboard** (monitoring and orchestration)

3. **Access the Application**
   - **Next.js UI**: Available at the URL shown in Aspire dashboard
   - **Aspire Dashboard**: https://localhost:17122
   - **GameSession API**: Injected automatically via Aspire
   - **GameEngine API**: Injected automatically via Aspire

## 🔧 Recent Improvements

### API Proxy Architecture (Latest)
- **Unified API endpoints** via Next.js proxy routes (`/api/game/*`)
- **SignalR WebSocket proxy** for real-time communication in containers
- **CORS-free container development** with seamless service communication
- **Automatic environment detection** for local vs container modes

### Frontend Enhancements
- **TypeScript improvements** with proper type safety for API responses
- **Enhanced error handling** for WebSocket connections
- **Real-time game state updates** with proper data mapping
- **Responsive UI** with modern React patterns

### Backend Improvements
- **Enhanced session response structure** with game tracking
- **Improved error handling** and logging
- **Better service discovery** in containerized environments
- **Optimized SignalR hub** for real-time updates

## 🐳 Container Development

### Building Docker Images

```bash
# Build all service images
docker build -f GameEngine.Dockerfile -t tictactoe-gameengine:local-test .
docker build -f GameSession.Dockerfile -t tictactoe-gamesession:local-test .
docker build -f WebUI.Dockerfile -t tictactoe-webui:local-test .
```

### Aspire Development Modes

The project supports two development modes through .NET Aspire:

| Mode | Command | Backend | Frontend | Use Case |
|------|---------|---------|----------|----------|
| **Default** | `dotnet run --project aspire/TicTacToe.AppHost` | .NET projects | Next.js dev | Daily development, hot reload |
| **Containers** | `dotnet run --project aspire/TicTacToe.AppHost -- --use-containers` | Container images | Container image | Production simulation |

### Development Mode Details

**Default Mode (Recommended for Development):**
- Fastest development experience
- Hot reload for both backend and frontend
- Automatic service discovery via Aspire
- Perfect for daily development and feature work

**Container Mode:**
- Full production simulation
- All services run as containers
- Uses Next.js API proxy routes for backend communication
- Useful for final testing before deployment

### API Proxy Architecture

In **Container Mode**, the Next.js frontend uses API proxy routes to communicate with backend services:

```
Frontend (Next.js) → API Routes (/api/game/*) → Backend Services
```

This architecture:
- **Eliminates CORS issues** in containerized environments
- **Provides unified API endpoints** regardless of deployment mode
- **Enables seamless switching** between development and container modes
- **Maintains security** by proxying requests through the frontend server

**Key Proxy Routes:**
- `/api/game/sessions/*` - Session management endpoints
- `/api/game/gameHub` - SignalR WebSocket proxy for real-time updates

## 🔧 Manual Service Setup (Legacy)

If you prefer to run services individually (not recommended):

1. **Start Backend Services**
   ```bash
   # Option A: Individual services
   dotnet run --project src/TicTacToe.GameEngine
   dotnet run --project src/TicTacToe.GameSession
   ```

2. **Configure Frontend**
   ```bash
   cd src/TicTacToe.WebUI
   npm install
   ```

3. **Start Frontend**
   ```bash
   npm run dev
   ```

**Note:** Manual setup requires additional configuration for service discovery and may not work with all features. Use .NET Aspire for the best development experience.

## 🏗️ Project Structure

```
TicTacToe/
├── aspire/                          # .NET Aspire orchestration
│   ├── TicTacToe.AppHost/          # Main application host
│   └── TicTacToe.ServiceDefaults/  # Shared service configuration
├── src/
│   ├── TicTacToe.GameEngine/       # Core game logic service
│   ├── TicTacToe.GameSession/      # Session management service
│   └── TicTacToe.WebUI/            # Next.js frontend application
│       ├── src/app/api/game/       # API proxy routes for backend communication
│       │   ├── [...path]/          # Dynamic API route for session endpoints
│       │   └── gameHub/            # SignalR WebSocket proxy
│       └── src/services/           # Frontend service layer
├── tests/                          # Comprehensive test suite
│   ├── TicTacToe.GameEngine.Tests/ # Backend unit & integration tests
│   ├── TicTacToe.GameSession.Tests/ # Backend unit & integration tests
│   └── TicTacToe.WebUI/tests/      # Frontend tests (unit, integration, E2E)
├── GameEngine.Dockerfile           # GameEngine container definition
├── GameSession.Dockerfile          # GameSession container definition
├── WebUI.Dockerfile                # Next.js production container
└── README.md                       # This file
```

## 🧪 Testing

The project includes comprehensive testing across all layers:

### Backend Testing

```bash
# Run all backend tests
dotnet test

# Run specific test projects
dotnet test tests/TicTacToe.GameEngine.Tests/
dotnet test tests/TicTacToe.GameSession.Tests/
```

### Frontend Testing

```bash
cd src/TicTacToe.WebUI

# Install dependencies (if not already done)
npm install

# Unit tests (Jest + React Testing Library)
npm test

# Unit tests with coverage
npm run test:coverage

# Run E2E tests (basic UI rendering test that doesn't require backend)
npm run test:e2e

# E2E tests with UI (interactive mode)
npm run test:e2e:ui

# E2E tests in headed mode (see browser)
npm run test:e2e:headed
```

### Testing Strategy

The project uses a **three-tier testing approach**:

1. **Unit Tests** - Fast, isolated testing of individual components and services
2. **Integration Tests** - API integration testing with mocked dependencies
3. **E2E Tests** - Full user journey testing in real browsers

For detailed frontend testing information, see [`src/TicTacToe.WebUI/TESTING.md`](src/TicTacToe.WebUI/TESTING.md).

## 🚀 Deployment

### Production Build

```bash
# Build production images
docker build -f GameEngine.Dockerfile -t tictactoe-gameengine:latest .
docker build -f GameSession.Dockerfile -t tictactoe-gamesession:latest .
docker build -f WebUI.Dockerfile -t tictactoe-webui:latest .
```

### Environment Configuration

The application uses .NET Aspire for environment variable injection and service discovery. In production, ensure:

- **Service URLs** are properly configured
- **CORS policies** are set for your domain
- **SignalR** endpoints are accessible
- **Health checks** are configured

## 🔧 Troubleshooting

### Common Issues

**WebSocket Connection Errors in Local Mode:**
- This is normal during development - SignalR tries WebSocket first, then falls back to Server-Sent Events (SSE)
- The application continues to work perfectly with SSE transport
- These errors don't appear in container mode due to proper proxy configuration
- In production, ensure proper WebSocket proxy configuration

**API 404 Errors:**
- Ensure you're using the correct proxy routes (`/api/game/*`)
- Check that containers are built with the latest code
- Verify Aspire is running the correct mode (local vs containers)

**TypeScript Build Errors:**
- Run `npm install` in the WebUI directory
- Clear Next.js cache: `rm -rf .next`
- Ensure all dependencies are up to date

**Container Build Failures:**
- Ensure Docker is running
- Check that all Dockerfiles are in the correct locations
- Verify the build context matches the Dockerfile expectations

**SignalR Connection Issues:**
- In local mode: Check browser console for connection details
- In container mode: Ensure the proxy routes are working
- Verify the GameSession service is healthy

### Getting Help

1. **Check the logs** in Aspire dashboard for detailed error information
2. **Verify service health** using the health check endpoints
3. **Test API endpoints** directly using the provided HTTP files
4. **Review the test suite** for examples of proper API usage

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📋 Third-Party Licenses

This project uses several third-party libraries with various open-source licenses:

- **Sharp image processing libraries** use LGPL-2.0, LGPL-2.1, LGPL-3.0, and MPL-2.0 licenses
- **caniuse-lite** uses CC-BY-4.0 license for browser compatibility data
- **tslib** uses 0BSD license

These libraries are used as dependencies and are not modified. Users may replace these libraries with their own versions as permitted by the respective licenses.

## 🙏 Acknowledgments

- Built with [.NET 9](https://dotnet.microsoft.com/)
- Frontend powered by [Next.js 15](https://nextjs.org/)
- Orchestrated by [.NET Aspire](https://dotnet.microsoft.com/aspire)
- Real-time communication via [SignalR](https://dotnet.microsoft.com/apps/aspnet/signalr)