# TicTacToe - Distributed Microservices Architecture

[![CI Pipeline](https://img.shields.io/github/actions/workflow/status/dmitrycx/TicTacToe/ci.yml?branch=main&style=for-the-badge)](https://github.com/dmitrycx/TicTacToe/actions)
[![.NET Version](https://img.shields.io/badge/.NET-9.0.301-blue?style=for-the-badge)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=for-the-badge)](LICENSE)

A production-ready demonstration of modern microservices architecture, featuring automated Tic Tac Toe gameplay with real-time visualization. Built with .NET 9, Next.js 15, and orchestrated by .NET Aspire.

## 🎯 Overview

This project showcases a distributed system where two AI players compete in Tic Tac Toe, with real-time game visualization through a modern web interface. The architecture demonstrates enterprise-grade patterns including:

- **Microservices Communication**: Service-to-service HTTP APIs
- **Real-time Updates**: SignalR WebSocket connections
- **API Gateway**: YARP reverse proxy with WebSocket support
- **Container Orchestration**: .NET Aspire for development and deployment
- **Modern Frontend**: Next.js 15 with TypeScript and real-time UI updates

## 🏗️ Architecture

### System Overview
```mermaid
graph TB
    subgraph "Client Layer"
        UI[Next.js UI]
    end
    
    subgraph "Gateway Layer"
        YARP[YARP API Gateway<br/>Reverse Proxy + WebSocket Proxy]
    end
    
    subgraph "Service Layer"
        GS[GameSession Service<br/>Session Orchestrator]
        GE[GameEngine Service<br/>Core Game Logic]
        SR[SignalR Hub<br/>Real-time Updates]
    end
    
    %% Client connections through gateway
    UI -->|HTTP: /api/game/sessions/*| YARP
    UI -->|HTTP: /api/game/engine/*| YARP
    UI -->|WebSocket: /api/game/gameHub| YARP
    
    %% Gateway proxy connections
    YARP -->|HTTP Proxy<br/>/*| GE
    YARP -->|WebSocket Proxy<br/>/gamehub| SR
    
    %% Service-to-Service communication
    GS -->|HTTP Client<br/>GameEngine API| GE
    GS -->|SignalR Events<br/>Broadcast Updates| SR

    style UI fill:#61DAFB
    style YARP fill:#FF6B35,color:#fff
    style GS fill:#512BD4,color:#fff
    style GE fill:#512BD4,color:#fff
    style SR fill:#FF6B35
```

### Service Responsibilities

| Service | Technology | Port | Responsibility |
|---------|------------|------|----------------|
| **YARP API Gateway** | .NET 9 + YARP | 8082 | Unified reverse proxy for HTTP API calls and WebSocket connections |
| **GameEngine** | .NET 9 + FastEndpoints | 8080 | Core game logic, board state, move validation |
| **GameSession** | .NET 9 + FastEndpoints + SignalR | 8081 | Session management, AI move orchestration, SignalR hub hosting |
| **WebUI** | Next.js 15 + React + TypeScript | 3000 | User interface with real-time updates |

## 🔄 Data Flow

### Game Session Flow (with YARP Proxy)
```mermaid
sequenceDiagram
    participant UI as Next.js UI
    participant YARP as YARP API Gateway
    participant GS as GameSession
    participant GE as GameEngine
    participant SR as SignalR Hub

    Note over UI,SR: 1. Session Creation
    UI->>YARP: POST /api/game/sessions (Create Session)
    YARP->>GS: Forward to /sessions
    GS->>GE: POST /games (Create Game)
    GE-->>GS: Game Created
    GS-->>YARP: Session Created
    YARP-->>UI: Session Response

    Note over UI,SR: 2. WebSocket Connection
    UI->>YARP: Connect WebSocket to /api/game/gameHub
    YARP->>SR: Proxy WebSocket to /gamehub
    SR-->>YARP: Connected
    YARP-->>UI: Connected

    Note over UI,SR: 3. Game Simulation Loop
    loop Game Simulation
        GS->>GE: POST /games/{id}/moves (AI Move)
        GE-->>GS: Move Applied
        GS->>SR: Broadcast GameUpdate
        SR->>YARP: Real-time Update
        YARP-->>UI: Real-time Update
    end
```

### Real-time Communication Flow
```mermaid
graph LR
    subgraph "Client Layer"
        UI[Next.js UI]
    end
    
    subgraph "Gateway Layer"
        YARP[YARP API Gateway<br/>HTTP + WebSocket Proxy]
    end
    
    subgraph "Service Layer"
        GS[GameSession Service]
        SR[SignalR Hub<br/>Hosted in GameSession]
    end
    
    %% Client connections through gateway
    UI -->|HTTP: /api/game/sessions/*| YARP
    UI -->|WebSocket: /api/game/gameHub| YARP
    
    %% Gateway proxy connections
    YARP -->|HTTP Proxy: /sessions/*| GS
    YARP -->|WebSocket Proxy: /gamehub| SR
    
    %% Internal service communication
    GS -->|SignalR Events| SR
    
    %% Real-time updates flow
    SR -->|Real-time Updates| YARP
    YARP -->|Real-time Updates| UI
    
    style UI fill:#61DAFB
    style YARP fill:#FF6B35,color:#fff
    style GS fill:#512BD4,color:#fff
    style SR fill:#FF6B35
```

### API Gateway Routing

```mermaid
graph TB
    subgraph "YARP API Gateway"
        subgraph "HTTP Routes"
            R1[game-session-api<br/>/api/game/sessions/* → /sessions/*]
            R2[game-engine-api<br/>/api/game/engine/* → /*]
        end
        
        subgraph "WebSocket Routes"
            R3[game-session-signalr<br/>/api/game/gameHub/* → /gamehub/*<br/>WebSocket: true]
        end
        
        subgraph "Clusters"
            C1[game-session-cluster<br/>→ game-session]
            C2[game-engine-cluster<br/>→ game-engine]
        end
    end
    
    R1 --> C1
    R2 --> C2
    R3 --> C1
    
    style R1 fill:#FF6B35,color:#fff
    style R2 fill:#FF6B35,color:#fff
    style R3 fill:#FF6B35,color:#fff
    style C1 fill:#512BD4,color:#fff
    style C2 fill:#512BD4,color:#fff
```

## 🎮 Game Features

### AI Strategies
- **Random**: Makes random valid moves
- **Rule-Based**: Uses basic Tic Tac Toe strategies
- **AI**: Advanced move prediction and optimization

### Real-time Features
- **Live Game Updates**: Real-time board state synchronization
- **Move Animation**: Smooth visual transitions
- **Game History**: Complete move-by-move replay
- **Session Management**: Multiple concurrent games

### Game Logic
- **Move Validation**: Ensures only valid moves are accepted
- **Win Detection**: Automatic game completion detection
- **Board State Management**: Maintains game integrity
- **Error Handling**: Graceful error recovery

## 🚀 Quick Start

### Prerequisites

- **.NET SDK 9.0.301** (see `global.json`)
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
   - **ApiGateway** (YARP reverse proxy, orchestrated by Aspire)
   - **Next.js UI** (dev server with hot reload)
   - **Aspire Dashboard** (monitoring and orchestration)

3. **Access the Application**
   - **Next.js UI**: Available at the URL shown in Aspire dashboard
   - **Aspire Dashboard**: https://localhost:17122
   - **GameSession API**: Injected automatically via Aspire
   - **GameEngine API**: Injected automatically via Aspire

## 📡 API Reference

### GameSession Service (`/api/game/sessions`)

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/game/sessions` | `POST` | Create a new game session |
| `/api/game/sessions` | `GET` | List all sessions |
| `/api/game/sessions/{id}` | `GET` | Get session details |
| `/api/game/sessions/{id}` | `DELETE` | Delete a session |
| `/api/game/sessions/{id}/simulate` | `POST` | Start game simulation |

### GameEngine Service (`/api/game/engine`)

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/game/engine/games` | `POST` | Create a new game |
| `/api/game/engine/games/{id}` | `GET` | Get game state |
| `/api/game/engine/games/{id}/moves` | `POST` | Make a move |

## 🚀 Future Enhancements Roadmap

This project provides a solid foundation for a production-grade application. The following roadmap outlines potential future improvements, categorized into key areas of development, from core functionality to enterprise-level operational readiness.

### Ⅰ. Foundational Improvements

These enhancements focus on making the application stateful, secure, and ready for user-specific features.

#### 1. Durable Persistence with Supabase

**Goal**: Replace the current in-memory data stores with a durable database to ensure that game and session data persists across application restarts.

**Action**:
- Implement SupabaseRepository classes in both the GameSession and GameEngine services.
- GameSession will be responsible for storing high-level session metadata.
- GameEngine will be responsible for storing the state of each individual game board.

**Benefit**: Transforms the application from a transient simulation into a stateful, production-ready system.

#### 2. Authentication & Authorization with JWTs

**Goal**: Secure the application by adding user accounts and protecting API endpoints.

**Action**:
- Evolve the GameSession service (or create a new AuthService) to handle user registration and login.
- Generate a JWT (JSON Web Token) upon successful login.
- Modify the API Gateway and backend services to validate the JWT on protected routes.

**Benefit**: Enables player-specific features like game history and leaderboards, while implementing a standard enterprise security pattern.

#### 3. Robust Configuration and Secrets Management

**Goal**: Remove sensitive information (database connection strings, JWT secrets, API keys) from source control.

**Action**:
- **Local Development**: Utilize .NET User Secrets.
- **CI/CD & Cloud**: Integrate with GitHub Actions Secrets and a cloud provider's secret store (e.g., Azure Key Vault, AWS Secrets Manager, Vercel Environment Variables).

**Benefit**: Critical for security and adherence to production best practices.

### Ⅱ. Feature Enhancements & User Experience

These features build upon the foundational services to create a more interactive and engaging application.

#### 4. Player vs. Player (P2P) and Spectator Modes

**Goal**: Allow two human players to compete against each other in real-time and let others watch.

**Action**:
- Enhance the SignalR hub to manage multiple client connections within a single game session.
- Implement turn-based logic, broadcasting game state updates only to the clients in that session.
- Create a "spectator" role that can receive updates but cannot make moves.

**Benefit**: Makes the application a fully interactive, multi-user experience.

#### 5. AI Agent-Based System (Advanced)

**Goal**: Refactor the AI logic from simple function calls into a true Multi-Agent System (MAS) for greater autonomy and extensibility.

**Action**:
- Create new "Player Agent" services, where each agent encapsulates a specific strategy (e.g., MinMax Agent, RuleBased Agent).
- Implement an Agent Communication Channel (ACC) using a message broker like RabbitMQ.
- The GameSession service becomes an "environment controller," sending requests to agents using a formal Agent Communication Language (like FIPA-ACL).

**Benefit**: Demonstrates advanced AI and distributed systems concepts, allowing for more complex, decoupled, and even polyglot (multi-language) agent implementations.

**Updated Architecture with Multi-Agent System**:
```mermaid
graph TD
   subgraph "Client & Gateway"
      UI[Next.js UI] --> YARP[YARP API Gateway]
   end

   subgraph "Orchestration & Game Logic"
      YARP --> GS[GameSession Service]
      GS --> GE[GameEngine Service]
   end

   subgraph "Multi-Agent System (MAS)"
      A1[Player Agent 'X'] -->|e.g., MinMax Strategy| AC
      A2[Player Agent 'O'] -->|e.g., RuleBased Strategy| AC
      AC[Agent Communication Channel] -->|e.g., RabbitMQ or SignalR| AC
   end

   subgraph "Communication"
      GS -->|"REQUEST: MakeMove"| AC
      AC -->|"FIPA-ACL: request(action=make-move)"| A1
      A1 -->|Reasoning & Decision| A1
      A1 -->|"INFORM: MoveComputed"| AC
      AC -->|Standard Event| GS
      GS -->|HTTP| GE
      GE -->|Game State| GS
      GS -->|SignalR| UI
   end

   style A1 fill:#00A6ED
   style A2 fill:#00A6ED
   style AC fill:#FFA500

```

### Ⅲ. Operability & Monitoring

These improvements focus on making the system observable, resilient, and easy to manage in production.

#### 6. Centralized Logging & Tracing

**Goal**: Gain deep insight into system behavior for debugging and performance analysis.

**Action**:
- **Structured Logging**: Implement Serilog in all .NET services to write JSON-formatted logs. This makes them searchable and easy to parse.
- **Log Aggregation**: In a production environment, ship these logs to a centralized platform like the ELK Stack (Elasticsearch, Logstash, Kibana) or Grafana Loki.
- **Distributed Tracing**: Leverage the built-in OpenTelemetry support from .NET Aspire to trace requests as they flow through all microservices, identifying bottlenecks instantly.

#### 7. Metrics & BI Dashboards

**Goal**: Visualize the health, performance, and business value of the application in real-time.

**Action**:
- **Application Metrics**: Add a Prometheus container to scrape the `/metrics` endpoints automatically exposed by Aspire services.
- **System Dashboards**: Add a Grafana container to create dashboards that visualize Prometheus metrics (e.g., requests per second, error rates, CPU/memory usage).
- **Business Intelligence**: Add a Metabase or Apache Superset container. Connect it directly to the Supabase database to build business-oriented dashboards (e.g., "Which AI strategy wins most often?", "Player engagement over time").

**Updated Architecture with Observability & BI**:
```mermaid
graph TD
    subgraph "Application Stack"
        UI[Next.js UI] --> YARP[API Gateway] --> Services[Microservices] --> DB[(Supabase DB)]
    end

    subgraph "Observability Stack"
        Prometheus[Prometheus] --> Services
        Grafana[Grafana] --> Prometheus
        ELK[ELK / Loki] --> Services
    end

    subgraph "Analytics Stack"
        BI[Metabase / Superset] -->|SQL Query| DB
    end

    style Prometheus fill:#E6522C,color:white
    style Grafana fill:#F46800,color:white
    style ELK fill:#005571,color:white
    style BI fill:#F96D00,color:white
```

#### 8. Resilience & Fault Tolerance Patterns

**Goal**: Ensure the system can handle transient errors and service outages gracefully.

**Action**:
- Implement advanced Polly resilience policies for all inter-service HTTP calls.
- **Circuit Breaker**: Automatically stop sending requests to a service that is repeatedly failing.
- **Timeout**: Prevent a slow service from causing cascading delays.
- **Deep Health Checks**: Enhance the existing `/alive` endpoints to check not just the service itself, but its critical dependencies (like its database connection), providing a true health status.

### Ⅳ. Deployment & Scalability

These steps focus on getting the application into the cloud and ensuring it can handle increased load.

#### 9. Full CI/CD (Continuous Deployment)

**Goal**: Automate the deployment process so that every merge to the main branch is safely deployed to production.

**Action**:
- Enhance the GitHub Actions workflow to include a deploy job.
- This job will build and push the final Docker images to a container registry (e.g., Docker Hub, Azure Container Registry).
- It will then run infrastructure-as-code scripts (e.g., Bicep, Terraform, Pulumi) to update the cloud services with the new container images.

#### 10. Cloud Deployment & Horizontal Scaling

**Goal**: Host the application in a scalable cloud environment.

**Action**:
- **Frontend**: Deploy the Next.js UI to a specialized host like Vercel.
- **Backend**: Deploy all .NET services (API Gateway, GameSession, GameEngine) as containers to a managed service like Azure Container Apps or AWS Fargate.
- Configure auto-scaling rules based on CPU usage or incoming request count to automatically add more container instances (replicas) under heavy load.

## 🧪 Quick Testing

### **Fast Testing Commands**
```bash
# Unit Tests (Fast - In-Memory)
dotnet test --filter "Category=Unit"

# Integration Tests (In-Memory)
dotnet test --filter "Category=Integration"

# Frontend Tests
cd src/TicTacToe.WebUI && npm test

# Complete Test Suite
npm run test:all
```

**For detailed testing commands, see [Testing Commands](docs/testing/README.md).**

## 🏗️ Project Structure

```
TicTacToe/
├── aspire/                          # .NET Aspire orchestration
│   ├── TicTacToe.AppHost/          # Main application host
│   └── TicTacToe.ServiceDefaults/  # Shared service configuration
├── docs/                           # 📚 Organized documentation
│   ├── setup/                      # Setup guides
│   ├── development/                # Development guides
│   ├── testing/                    # Testing documentation
│   ├── deployment/                 # Deployment guides
│   └── architecture/               # Architecture documentation
├── scripts/                        # 🔧 Organized utility scripts
│   ├── testing/                    # Testing scripts
│   ├── development/                # Development scripts
│   └── deployment/                 # Deployment scripts
├── src/
│   ├── TicTacToe.GameEngine/       # Core game logic service
│   │   ├── Domain/                 # Game domain models
│   │   ├── Endpoints/              # API endpoints
│   │   └── Persistence/            # Data access layer
│   ├── TicTacToe.GameSession/      # Session management service
│   │   ├── Domain/                 # Session domain models
│   │   ├── Endpoints/              # API endpoints
│   │   ├── Hubs/                   # SignalR hubs
│   │   └── Services/               # Business logic
│   ├── TicTacToe.ApiGateway/       # YARP reverse proxy
│   │   ├── Program.cs              # Gateway configuration
│   │   └── appsettings.json       # Routing and CORS config
│   └── TicTacToe.WebUI/            # Next.js frontend application
│       ├── src/
│       │   ├── app/                # Next.js app router
│       │   ├── components/         # React components
│       │   ├── services/           # API clients
│       │   └── types/              # TypeScript types
│       └── tests/                  # Frontend tests
├── tests/                          # Comprehensive test suite
│   ├── TicTacToe.GameEngine.Tests/ # Backend unit & integration tests
│   └── TicTacToe.GameSession.Tests/ # Backend unit & integration tests
├── GameEngine.Dockerfile           # GameEngine container definition
├── GameSession.Dockerfile          # GameSession container definition
├── ApiGateway.Dockerfile           # ApiGateway container definition
├── WebUI.Dockerfile                # Next.js production container
└── README.md                       # This file
```

## 🔧 Technology Stack

### Backend
- **.NET 9**: Latest .NET framework
- **FastEndpoints**: High-performance API framework
- **SignalR**: Real-time communication
- **YARP**: Reverse proxy and load balancer
- **.NET Aspire**: Application orchestration

### Frontend
- **Next.js 15**: React framework with app router
- **TypeScript**: Type-safe JavaScript
- **Tailwind CSS**: Utility-first CSS framework
- **Playwright**: End-to-end testing

### Development & Testing
- **Docker**: Containerization
- **xUnit**: Unit testing framework
- **Jest**: JavaScript testing
- **GitHub Actions**: CI/CD pipeline

## 📊 Performance Characteristics

| Metric | Value | Notes |
|--------|-------|-------|
| **Startup Time** | < 5 seconds | All services ready |
| **API Response Time** | < 100ms | Average response time |
| **WebSocket Latency** | < 50ms | Real-time updates |
| **Memory Usage** | ~200MB | Total application |
| **Test Execution** | < 10 seconds | Full test suite |

## 🚨 Troubleshooting

### Common Issues

**Service Won't Start:**
```bash
# Check .NET version
dotnet --version  # Should be 9.0.301

# Clean and rebuild
dotnet clean && dotnet build
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

**CORS Issues:**
```bash
# Check if frontend origin is in ApiGateway CORS config
# Update src/TicTacToe.ApiGateway/appsettings.json if needed
```

## 📚 Documentation

For comprehensive documentation, see the **[Documentation Hub](docs/README.md)** which includes:

### **🚀 Getting Started**
- **[Setup Guide](docs/setup/README.md)** - Complete setup instructions
- **[Quick Start](docs/setup/quick-start.md)** - Get running in 5 minutes
- **[Requirements](docs/setup/requirements.md)** - System requirements

### **👨‍💻 Development**
- **[Developer Guide](docs/development/README.md)** - Development workflow
- **[Testing Strategy](docs/development/testing-strategy.md)** - Testing approach

### **🧪 Testing**
- **[Testing Commands](docs/testing/commands.md)** - Quick reference for test commands
- **[Testing Guide](docs/testing/README.md)** - Complete testing documentation

### **🚀 Deployment**
- **[Deployment Guide](docs/deployment/README.md)** - Production deployment
- **[Container Setup](docs/deployment/containers.md)** - Docker configuration

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

### Development Guidelines
- Follow the existing code style and patterns
- Add unit tests for new functionality
- Update documentation for API changes
- Test both backend and frontend changes

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
- Frontend powered by [Next.js](https://nextjs.org/)
- Real-time communication via [SignalR](https://dotnet.microsoft.com/apps/aspnet/signalr)
- Container orchestration with [.NET Aspire](https://dotnet.microsoft.com/cloud-native/aspire)