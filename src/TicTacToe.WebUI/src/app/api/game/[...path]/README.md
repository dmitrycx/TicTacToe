# API Endpoint: /api/game/[...path]

This directory contains the dynamic API route that proxies all game-related requests to the backend services.

## `route.ts`

### Dynamic Route: `/api/game/{path}`

- **Description:** Dynamic proxy route that forwards requests to the GameSession microservice
- **Path Parameter:** `path` - The specific endpoint path to forward
- **Supported Methods:** GET, POST, PUT, DELETE
- **Target Service:** GameSession microservice

### Supported Endpoints

The dynamic route supports all GameSession service endpoints:

- **`GET /api/game/sessions`** → `GET {GAME_SESSION_SERVICE_URL}/sessions`
- **`POST /api/game/sessions`** → `POST {GAME_SESSION_SERVICE_URL}/sessions`
- **`GET /api/game/sessions/{sessionId}`** → `GET {GAME_SESSION_SERVICE_URL}/sessions/{sessionId}`
- **`DELETE /api/game/sessions/{sessionId}`** → `DELETE {GAME_SESSION_SERVICE_URL}/sessions/{sessionId}`
- **`POST /api/game/sessions/{sessionId}/simulate`** → `POST {GAME_SESSION_SERVICE_URL}/sessions/{sessionId}/simulate`

### Features

- **Dynamic Routing:** Automatically forwards any path under `/api/game/` to the backend service
- **Method Preservation:** Maintains the original HTTP method (GET, POST, etc.)
- **Header Forwarding:** Preserves request headers for authentication and content negotiation
- **Error Handling:** Provides consistent error responses for network issues
- **Environment Detection:** Automatically uses the correct service URL based on deployment mode

### Implementation Details

The route uses Next.js dynamic routing with `[...path]` to capture all segments and forward them to the backend service. This provides a flexible proxy that can handle any endpoint without requiring individual route files.

### Status Codes

- **200/201**: Successfully forwarded and received response
- **400**: Bad request (forwarded from backend)
- **404**: Resource not found (forwarded from backend)
- **409**: Conflict (forwarded from backend)
- **500**: Internal server error or network issue 