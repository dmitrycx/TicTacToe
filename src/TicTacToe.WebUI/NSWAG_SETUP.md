# NSwag Setup and Workflow

This document describes the NSwag setup for automatic TypeScript client generation from the .NET backend APIs.

## Overview

NSwag automatically generates TypeScript clients from your .NET FastEndpoints APIs, ensuring type safety and eliminating manual synchronization between frontend and backend.

## Architecture

```
.NET Backend (FastEndpoints) → NSwag → OpenAPI/Swagger → TypeScript Client
```

## Backend Configuration

### 1. NuGet Packages Added
- `NSwag.AspNetCore` - Provides OpenAPI generation and Swagger UI

### 2. Program.cs Changes
Both `GameSession` and `GameEngine` services now use NSwag instead of FastEndpoints Swagger:

```csharp
// Configure NSwag for OpenAPI generation
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "TicTacToe GameSession API";
    settings.Description = "API for managing TicTacToe game sessions and simulations";
    settings.Version = "v1";
    
    // Include XML documentation
    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
    if (File.Exists(xmlPath))
    {
        settings.DocumentProcessors.Add(new NSwag.Generation.Processors.DocumentProcessor(xmlPath));
    }
});

// NSwag middleware
app.UseOpenApi(); // Serves the openapi.json file
app.UseSwaggerUi(); // Serves the interactive Swagger UI page
```

### 3. XML Documentation
- XML documentation is already enabled in both projects
- Use `/// <summary>` comments in your endpoints and DTOs for better API documentation

## Frontend Configuration

### 1. NSwag CLI Installation
```bash
npm install nswag --save-dev
```

### 2. Configuration File
`nswag.json` - Configures the code generation process:
- Source: `http://localhost:37053/swagger/v1/swagger.json` (GameSession API)
- Output: `src/services/generated-client.ts`
- Template: Fetch-based TypeScript client

### 3. NPM Script
```json
{
  "scripts": {
    "generate-api": "nswag run nswag.json"
  }
}
```

## Workflow

### 1. Start Backend Services
Ensure your .NET services are running:
```bash
# From the root directory
dotnet run --project aspire/TicTacToe.AppHost
```

### 2. Generate TypeScript Client
```bash
npm run generate-api
```

This will:
- Fetch the OpenAPI specification from the running service
- Generate a fully typed TypeScript client
- Create all necessary interfaces and types
- Output to `src/services/generated-client.ts`

### 3. Use Generated Client
The generated client is wrapped in `src/services/api-client.ts` for easier use:

```typescript
import { apiClient } from './services/api-client';

// Create a session
const session = await apiClient.createSession('Player1', 'Player2', 'HumanVsHuman');

// List sessions
const sessions = await apiClient.listSessions();

// Get specific session
const sessionDetails = await apiClient.getSession(sessionId);

// Simulate game
const simulation = await apiClient.simulateGame(sessionId, 'Random');
```

## Benefits

### 1. Single Source of Truth
- API contract is defined in the .NET backend
- Frontend types are automatically generated
- No manual synchronization required

### 2. Type Safety
- Full TypeScript support with proper types
- Compile-time checking of API calls
- IntelliSense support in your IDE

### 3. Automatic Updates
- When you change a DTO or endpoint in the backend
- Just re-run `npm run generate-api`
- Frontend types are automatically updated

### 4. Professional Documentation
- Interactive Swagger UI at `/swagger`
- OpenAPI specification at `/swagger/v1/swagger.json`
- XML comments are included in the documentation

## Adding New Endpoints

1. **Backend**: Add new FastEndpoint with proper XML documentation
2. **Generate**: Run `npm run generate-api`
3. **Frontend**: Use the new generated methods in your components

## Troubleshooting

### Service Not Running
If you get connection errors during generation:
```bash
# Check if the service is running
curl http://localhost:37053/swagger/v1/swagger.json
```

### Generated Client Issues
If the generated client has issues:
1. Check the OpenAPI spec at `/swagger/v1/swagger.json`
2. Verify XML documentation is properly formatted
3. Regenerate with `npm run generate-api`

### Type Conflicts
If you have existing types that conflict:
1. Remove old manual type definitions
2. Use only the generated types
3. Update imports to use generated types

## Future Enhancements

1. **Multiple Services**: Add GameEngine API generation
2. **Build Integration**: Add generation to build process
3. **Validation**: Add runtime validation using generated schemas
4. **Testing**: Generate test stubs from the API specification 