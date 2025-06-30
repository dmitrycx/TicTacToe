# API Endpoint: /api/game/sessions

This directory handles API routes related to the root of the sessions resource through the proxy architecture.

## `route.ts`

### `GET /api/game/sessions`

- **Description:** Retrieves a list of all game sessions.
- **Acts as a BFF for:** `GET {GAME_SESSION_SERVICE_URL}/sessions`
- **Response:** `ListSessionsResponse` containing an array of session summaries
- **Status Codes:**
  - `200`: Successfully retrieved sessions
  - `500`: Internal server error

### `POST /api/game/sessions`

- **Description:** Creates a new game session with specified strategy.
- **Acts as a BFF for:** `POST {GAME_SESSION_SERVICE_URL}/sessions`
- **Request Body:** `CreateSessionRequest` with strategy selection
- **Response:** `CreateSessionResponse` with session details
- **Status Codes:**
  - `201`: Session created successfully
  - `400`: Invalid request data
  - `500`: Internal server error

## Purpose

This endpoint serves as a Backend for Frontend (BFF) layer that:
- Proxies requests to the GameSession microservice through `/api/game/sessions`
- Handles CORS and authentication (if needed in the future)
- Provides a unified API interface for the frontend
- Transforms responses as needed for the UI
- Works seamlessly in both local and container modes 