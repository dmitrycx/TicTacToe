# Developer Guide

Technical guide for developers working on the TicTacToe project.

## 🏗️ **Architecture Decisions**

### **Why .NET 9?**
- **Latest Features**: Top-level statements, record types, pattern matching
- **Performance**: High-performance runtime with minimal allocations
- **Ecosystem**: Rich ecosystem with FastEndpoints, SignalR, and Aspire
- **Future-Proof**: Long-term support and continuous improvements

### **Why FastEndpoints?**
- **Performance**: Minimal overhead compared to traditional controllers
- **Type Safety**: Strong typing with source generators
- **Validation**: Built-in validation with FluentValidation
- **Documentation**: Auto-generated OpenAPI/Swagger documentation
- **Testing**: Excellent testing support with FastEndpoints.Testing

### **Why SignalR?**
- **Real-time**: Bidirectional communication for live updates
- **Scalability**: Built-in support for multiple transport protocols
- **Integration**: Seamless integration with ASP.NET Core
- **Reliability**: Automatic reconnection and connection management

### **Why YARP?**
- **Performance**: High-performance reverse proxy
- **Flexibility**: Dynamic configuration and routing
- **WebSocket Support**: Native WebSocket proxying
- **Load Balancing**: Built-in load balancing capabilities

### **Why .NET Aspire?**
- **Orchestration**: Simplified multi-service development
- **Observability**: Built-in monitoring and logging
- **Service Discovery**: Automatic service registration and discovery
- **Development Experience**: Hot reload for all services

### **Why Next.js 15?**
- **App Router**: Modern file-based routing
- **TypeScript**: First-class TypeScript support
- **Performance**: Server-side rendering and optimization
- **Developer Experience**: Excellent hot reload and debugging

## 🔧 **Development Workflow**

### **Daily Development**
```bash
# 1. Start services with hot reload
dotnet run --project aspire/TicTacToe.AppHost

# 2. Make changes (hot reload enabled)

# 3. Run unit tests frequently
dotnet test --filter "Category=Unit"

# 4. Run integration tests before commits
dotnet test --filter "Category=Integration"
```

### **Code Organization**

#### **Backend Services**
```
src/TicTacToe.GameEngine/
├── Domain/           # Domain models and business logic
├── Endpoints/        # API endpoints (FastEndpoints)
└── Persistence/      # Data access layer

src/TicTacToe.GameSession/
├── Domain/           # Domain models and business logic
├── Endpoints/        # API endpoints (FastEndpoints)
├── Hubs/             # SignalR hubs
└── Services/         # Business logic services
```

#### **Frontend Application**
```
src/TicTacToe.WebUI/
├── src/
│   ├── app/          # Next.js app router pages
│   ├── components/   # React components
│   ├── services/     # API clients and utilities
│   └── types/        # TypeScript type definitions
└── tests/            # Frontend tests
```

### **API Design Patterns**

#### **FastEndpoints Pattern**
```csharp
public class CreateGameEndpoint : Endpoint<CreateGameRequest, CreateGameResponse>
{
    public override void Configure()
    {
        Post("/games");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateGameRequest req, CancellationToken ct)
    {
        var game = Game.Create();
        var savedGame = await _repository.SaveAsync(game);
        
        await SendAsync(new CreateGameResponse { Id = savedGame.Id });
    }
}
```

#### **SignalR Hub Pattern**
```csharp
public class GameHub : Hub
{
    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }
}
```

## 🎯 **Best Practices**

### **Backend Development**
- **Domain-Driven Design**: Keep business logic in domain models
- **Dependency Injection**: Use constructor injection for services
- **Async/Await**: Use async patterns consistently
- **Error Handling**: Implement proper exception handling
- **Logging**: Use structured logging with appropriate levels

### **Frontend Development**
- **TypeScript**: Use strict TypeScript configuration
- **Component Composition**: Build reusable components
- **State Management**: Use React hooks for local state
- **API Integration**: Use typed API clients
- **Error Boundaries**: Implement error boundaries for graceful failures

### **Testing Strategy**
- **Unit Tests**: Test individual components and methods
- **Integration Tests**: Test service interactions
- **E2E Tests**: Test complete user workflows
- **Test Data**: Use consistent test data and fixtures

## 🔍 **Debugging**

### **Backend Debugging**
```bash
# Check service logs in Aspire dashboard
# Open https://localhost:17122

# Test API endpoints directly
curl -X POST http://localhost:8081/sessions \
  -H "Content-Type: application/json" \
  -d '{"strategy": "Random"}'

# Check service health
curl http://localhost:8080/health
```

### **Frontend Debugging**
```bash
# Start frontend in development mode
cd src/TicTacToe.WebUI
npm run dev

# Use browser dev tools for debugging
# Check Network tab for API calls
# Check Console for errors
```

### **SignalR Debugging**
```bash
# Check WebSocket connections
# Use browser dev tools Network tab
# Look for WebSocket frames
```

## 📊 **Performance Considerations**

### **Backend Performance**
- **FastEndpoints**: Minimal overhead for API endpoints
- **In-Memory Repositories**: Fast data access for development
- **Async Operations**: Non-blocking I/O operations
- **Caching**: Consider caching for frequently accessed data

### **Frontend Performance**
- **Next.js Optimization**: Automatic code splitting and optimization
- **Bundle Analysis**: Monitor bundle size and performance
- **Image Optimization**: Use Next.js image optimization
- **Lazy Loading**: Implement lazy loading for components

## 🚀 **Deployment Considerations**

### **Containerization**
- **Multi-stage Builds**: Optimize container images
- **Health Checks**: Implement proper health checks
- **Environment Variables**: Use environment-specific configuration
- **Resource Limits**: Set appropriate resource limits

### **Monitoring**
- **Logging**: Structured logging with correlation IDs
- **Metrics**: Application metrics and performance monitoring
- **Tracing**: Distributed tracing for request flows
- **Alerting**: Set up alerts for critical issues

## 📚 **Related Documentation**

- **[Setup Guide](../setup/README.md)** - Environment setup
- **[Testing Strategy](testing-strategy.md)** - Testing approach
- **[API Documentation](../architecture/api-design.md)** - API design patterns
- **[Deployment Guide](../deployment/README.md)** - Production deployment 