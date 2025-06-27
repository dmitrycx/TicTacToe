# TicTacToe BFF API Documentation

This directory contains the Backend for Frontend (BFF) API routes for the TicTacToe application. The BFF acts as an intermediary layer between the frontend and the microservices, providing a unified API interface.

## Architecture Overview

```
Frontend (Next.js) → BFF API Routes → Microservices
                                    ├── GameSession Service
                                    └── GameEngine Service
```

## BFF Benefits

- **CORS Management**: Handles cross-origin requests without exposing microservices directly
- **API Aggregation**: Combines multiple microservice calls into single endpoints when needed
- **Response Transformation**: Adapts microservice responses for frontend consumption
- **Error Handling**: Provides consistent error responses across all endpoints
- **Security**: Acts as a security boundary between frontend and microservices

## Available Endpoints

### Sessions Management

#### `GET /api/sessions`
Retrieves a list of all game sessions.

#### `POST /api/sessions`
Creates a new game session with specified players and game type.

#### `GET /api/sessions/{sessionId}`
Retrieves details of a specific game session by its ID.

#### `DELETE /api/sessions/{sessionId}`
Deletes a specific game session by its ID.

#### `POST /api/sessions/{sessionId}/simulate`
Initiates a game simulation for a specific session.

## Environment Variables

The BFF uses the following environment variables:

- `GAME_SESSION_SERVICE_URL`: URL of the GameSession microservice
- `GAME_ENGINE_SERVICE_URL`: URL of the GameEngine microservice (if needed in the future)

## Error Handling

All endpoints follow a consistent error handling pattern:

- **200/201**: Success responses
- **400**: Bad request (invalid input)
- **404**: Resource not found
- **409**: Conflict (e.g., session in invalid state)
- **500**: Internal server error

## Development

### Adding New Endpoints

1. Create a new route file in the appropriate directory
2. Add JSDoc comments with `@swagger` annotations
3. Create a README.md file in the endpoint directory
4. Update this main README.md with the new endpoint

### Testing

Test endpoints using the provided HTTP files or tools like Postman:

- `TicTacToe.GameSession.http` - GameSession service endpoints
- `TicTacToe.GameEngine.http` - GameEngine service endpoints

## Future Enhancements

- **Authentication**: Add JWT token validation
- **Rate Limiting**: Implement request throttling
- **Caching**: Add response caching for frequently accessed data
- **Logging**: Enhanced request/response logging
- **Metrics**: Add performance monitoring 