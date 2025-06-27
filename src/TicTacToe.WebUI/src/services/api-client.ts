import { GameSessionClient, TicTacToeGameSessionEndpointsSimulateGameRequest } from './generated-client';

// Create a singleton instance of the generated client
const gameSessionClient = new GameSessionClient();

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
  async createSession(player1Name: string, player2Name: string, gameType: string) {
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
        request.moveStrategy = moveStrategy as any;
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