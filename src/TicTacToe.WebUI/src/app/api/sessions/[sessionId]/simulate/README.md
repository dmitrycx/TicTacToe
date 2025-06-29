# API Endpoint: /api/game/sessions/[sessionId]/simulate

This directory handles API routes related to game simulation within a specific session through the proxy architecture.

## `route.ts`

### `POST /api/game/sessions/{sessionId}/simulate`

- **Description:** Initiates a game simulation for a specific session, allowing the session to play moves automatically.
- **Acts as a BFF for:** `POST {GAME_SESSION_SERVICE_URL}/sessions/{sessionId}/simulate`
- **Parameters:**
  - `sessionId` (path): The unique identifier of the session to simulate (UUID format)
- **Request Body:** `SimulateGameRequest` (optional parameters for simulation control)
- **Response:** `SimulateGameResponse` with simulation results and final game state
- **Status Codes:**
  - `200`: Simulation completed successfully
  - `400`: Invalid simulation request
  - `404`: Session not found
  - `409`: Session is not in a valid state for simulation
  - `500`: Internal server error

## Purpose

This endpoint serves as a Backend for Frontend (BFF) layer that:
- Proxies simulation requests to the GameSession microservice through `/api/game/sessions/{sessionId}/simulate`
- Handles simulation-specific routing and parameter validation
- Provides a unified API interface for game simulation
- Transforms simulation responses as needed for the UI
- Manages real-time updates through SignalR connections
- Works seamlessly in both local and container modes 