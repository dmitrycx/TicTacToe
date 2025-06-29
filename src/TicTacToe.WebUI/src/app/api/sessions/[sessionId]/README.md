# API Endpoint: /api/game/sessions/[sessionId]

This directory handles API routes related to specific game sessions identified by their session ID through the proxy architecture.

## `route.ts`

### `GET /api/game/sessions/{sessionId}`

- **Description:** Retrieves details of a specific game session by its ID.
- **Acts as a BFF for:** `GET {GAME_SESSION_SERVICE_URL}/sessions/{sessionId}`
- **Parameters:**
  - `sessionId` (path): The unique identifier of the session (UUID format)
- **Response:** `GetSessionResponse` with complete session details including game state
- **Status Codes:**
  - `200`: Successfully retrieved session
  - `404`: Session not found
  - `500`: Internal server error

### `DELETE /api/game/sessions/{sessionId}`

- **Description:** Deletes a specific game session by its ID.
- **Acts as a BFF for:** `DELETE {GAME_SESSION_SERVICE_URL}/sessions/{sessionId}`
- **Parameters:**
  - `sessionId` (path): The unique identifier of the session to delete (UUID format)
- **Response:** No content on success
- **Status Codes:**
  - `204`: Session deleted successfully
  - `404`: Session not found
  - `500`: Internal server error

## Purpose

This endpoint serves as a Backend for Frontend (BFF) layer that:
- Proxies requests to the GameSession microservice for specific session operations through `/api/game/sessions/{sessionId}`
- Handles session-specific routing and parameter validation
- Provides a unified API interface for session management
- Transforms responses as needed for the UI
- Works seamlessly in both local and container modes 