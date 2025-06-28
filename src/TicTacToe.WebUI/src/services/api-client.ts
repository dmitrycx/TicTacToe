import { GameSessionClient, TicTacToeGameSessionEndpointsSimulateGameRequest, TicTacToeGameSessionDomainEnumsMoveType } from './generated-client';

// Get the service URL from environment variables (Aspire-injected)
const getServiceUrl = () => {
  if (typeof window !== 'undefined') {
    // Client-side: use the environment variable exposed by Next.js
    return process.env.NEXT_PUBLIC_GAME_SESSION_SERVICE_URL;
  } else {
    // Server-side: use the environment variable directly
    return process.env.NEXT_PUBLIC_GAME_SESSION_SERVICE_URL;
  }
};

// Create a singleton instance of the generated client with the correct service URL
const gameSessionClient = new GameSessionClient(getServiceUrl());

// Simplified API client that wraps the generated client
export class ApiClient {
  private static instance: ApiClient;
  private gameSessionClient: GameSessionClient;

  private constructor() {
    this.gameSessionClient = gameSessionClient;
  }

  public static getInstance(): ApiClient {
    if (!ApiClient.instance) {
      ApiClient.instance = new ApiClient();
    }
    return ApiClient.instance;
  }

  // Session Management
  async createSession() {
    try {
      const response = await this.gameSessionClient.ticTacToeGameSessionEndpointsCreateSessionEndpoint();
      return response;
    } catch (error) {
      console.error('Failed to create session:', error);
      throw error;
    }
  }

  async listSessions() {
    try {
      const response = await this.gameSessionClient.ticTacToeGameSessionEndpointsListSessionsEndpoint();
      return response;
    } catch (error) {
      console.error('Failed to list sessions:', error);
      throw error;
    }
  }

  async getSession(sessionId: string) {
    try {
      const response = await this.gameSessionClient.ticTacToeGameSessionEndpointsGetSessionEndpoint(sessionId);
      return response;
    } catch (error) {
      console.error('Failed to get session:', error);
      throw error;
    }
  }

  async deleteSession(sessionId: string) {
    try {
      const response = await this.gameSessionClient.ticTacToeGameSessionEndpointsDeleteSessionEndpoint(sessionId);
      return response;
    } catch (error) {
      console.error('Failed to delete session:', error);
      throw error;
    }
  }

  async simulateGame(sessionId: string, moveStrategy?: string) {
    try {
      const request = new TicTacToeGameSessionEndpointsSimulateGameRequest();
      if (moveStrategy) {
        request.moveStrategy = moveStrategy as TicTacToeGameSessionDomainEnumsMoveType;
      }
      const response = await this.gameSessionClient.ticTacToeGameSessionEndpointsSimulateGameEndpoint(sessionId, request);
      return response;
    } catch (error) {
      console.error('Failed to simulate game:', error);
      throw error;
    }
  }
}

// Export a singleton instance
export const apiClient = ApiClient.getInstance();

// Export types from the generated client for use in components
export type {
  TicTacToeGameSessionEndpointsCreateSessionResponse,
  TicTacToeGameSessionEndpointsListSessionsResponse,
  TicTacToeGameSessionEndpointsGetSessionResponse,
  TicTacToeGameSessionEndpointsDeleteSessionResponse,
  TicTacToeGameSessionEndpointsSimulateGameResponse,
  TicTacToeGameSessionEndpointsSessionSummary,
  TicTacToeGameSessionEndpointsMoveInfo,
  TicTacToeGameSessionDomainEnumsMoveType
} from './generated-client'; 