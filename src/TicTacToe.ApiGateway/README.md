# TicTacToe API Gateway

A production-ready API Gateway built with YARP (Yet Another Reverse Proxy) that provides unified access to all TicTacToe microservices.

## 🏗️ Architecture

The API Gateway acts as a single entry point for all client requests, providing:

- **Reverse Proxy** for HTTP API calls
- **WebSocket Support** for SignalR connections
- **Load Balancing** across service instances
- **Health Checks** for service monitoring
- **CORS Management** for cross-origin requests

## 🚀 Features

### HTTP API Proxying
- Routes `/api/game/sessions/*` to GameSession service
- Routes `/api/game/engine/*` to GameEngine service
- Preserves HTTP methods, headers, and request bodies
- Handles error responses and status codes

### WebSocket Support
- Proxies `/api/game/gameHub` to GameSession SignalR hub
- Handles WebSocket upgrade requests
- Forwards WebSocket headers and protocols
- Maintains bidirectional communication

### Health Monitoring
- Active health checks every 10 seconds
- Passive health monitoring with failure rate policies
- Automatic failover for unhealthy services
- Service reactivation after 1 minute of recovery

### Load Balancing
- Round-robin load balancing (default)
- Support for multiple service instances
- Automatic service discovery via Aspire

## 📋 Configuration

### Routes Configuration
```json
{
  "ReverseProxy": {
    "Routes": {
      "game-session-api": {
        "ClusterId": "game-session-cluster",
        "Match": {
          "Path": "/api/game/sessions/{**catch-all}"
        },
        "Transforms": [
          {
            "PathPattern": "/sessions/{**catch-all}"
          }
        ]
      },
      "game-session-signalr": {
        "ClusterId": "game-session-cluster",
        "Match": {
          "Path": "/api/game/gameHub/{**catch-all}"
        },
        "Transforms": [
          {
            "PathPattern": "/gamehub/{**catch-all}"
          }
        ]
      }
    }
  }
}
```

### Cluster Configuration
```json
{
  "Clusters": {
    "game-session-cluster": {
      "Destinations": {
        "game-session-1": {
          "Address": "http://game-session:8081"
        }
      },
      "HealthCheck": {
        "Active": {
          "Enabled": true,
          "Path": "/health",
          "Interval": "00:00:10",
          "Timeout": "00:00:05"
        }
      }
    }
  }
}
```

## 🔧 Development

### Local Development
```bash
# Run the API Gateway locally
dotnet run --project src/TicTacToe.ApiGateway

# The gateway will be available at http://localhost:8082
```

### Container Development
```bash
# Build the container
docker build -f ApiGateway.Dockerfile -t tictactoe-apigateway:local-test .

# Run with Aspire
dotnet run --project aspire/TicTacToe.AppHost -- --use-containers
```

## 🧪 Testing

### Health Check
```bash
curl http://localhost:8082/health
# Expected: "API Gateway is healthy"
```

### API Proxy Test
```bash
# Test session list endpoint
curl http://localhost:8082/api/game/sessions

# Test session creation
curl -X POST http://localhost:8082/api/game/sessions \
  -H "Content-Type: application/json" \
  -d '{"strategy": "Random"}'
```

### WebSocket Test
```bash
# Test WebSocket connection (using wscat or similar tool)
wscat -c ws://localhost:8082/api/game/gameHub
```

## 📊 Monitoring

### Health Check Endpoints
- `GET /health` - Gateway health status
- Service health checks are automatically performed

### Logging
- YARP logs are configured at Information level
- WebSocket connection logs are available
- Error logs include detailed proxy information

## 🔒 Security

### CORS Configuration
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Request Validation
- All requests are validated before proxying
- Malformed requests are rejected with appropriate status codes
- WebSocket upgrade requests are properly validated

## 🚀 Production Deployment

### Environment Variables
- `ASPNETCORE_ENVIRONMENT` - Set to "Production"
- Service URLs are injected by Aspire in container mode
- Health check paths can be customized per service

### Scaling
- Multiple API Gateway instances can be deployed
- Load balancer can distribute traffic across gateways
- Service discovery handles backend service scaling

## 🔄 Migration from BFF

### Before (Next.js BFF)
```
Frontend → Next.js API Routes → Backend Services
```

### After (YARP Gateway)
```
Frontend → YARP API Gateway → Backend Services
```

### Benefits
- ✅ **Production-grade proxy** with YARP
- ✅ **Native WebSocket support** for SignalR
- ✅ **Built-in load balancing** and health checks
- ✅ **Better performance** and scalability
- ✅ **Simplified frontend** (no proxy routes needed)

## 📚 Related Documentation

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [SignalR WebSocket Support](https://docs.microsoft.com/en-us/aspnet/core/signalr/websockets)
- [.NET Aspire Service Discovery](https://docs.microsoft.com/en-us/dotnet/aspire/fundamentals/service-discovery) 