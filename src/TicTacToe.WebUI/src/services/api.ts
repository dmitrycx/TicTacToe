import { 
  CreateSessionRequest, 
  CreateSessionResponse, 
  SimulateGameResponse,
  GameSession,
  GameStatus,
  Player,
  GameStrategy
} from '@/types/game'
import { apiClient } from './api-client'

export interface StrategyInfo {
  name: string;
  displayName: string;
  description: string;
}

export interface ListStrategiesResponse {
  strategies: StrategyInfo[];
}

// Extended interface to include strategy property
interface ExtendedGetSessionResponse {
  sessionId?: string;
  gameId?: string;
  status?: string;
  strategy?: GameStrategy;
  createdAt?: Date;
  startedAt?: Date | undefined;
  completedAt?: Date | undefined;
  moves?: Array<{
    row?: number;
    column?: number;
    player?: string;
  }>;
  winner?: string | undefined;
  result?: string | undefined;
}

export class ApiService {
  static async createSession(request: CreateSessionRequest): Promise<CreateSessionResponse> {
    // Only run on client side
    if (typeof window === 'undefined') {
      throw new Error('createSession can only be called on the client side');
    }
    
    const response = await apiClient.createSession(request.strategy)
    return {
      sessionId: response.sessionId || '',
      currentGameId: undefined,
      gameIds: [],
      status: (response.status as GameStatus) || 'waiting',
      strategy: request.strategy,
      moves: []
    }
  }

  static async simulateGame(sessionId: string): Promise<SimulateGameResponse> {
    // Only run on client side
    if (typeof window === 'undefined') {
      throw new Error('simulateGame can only be called on the client side');
    }
    
    const response = await apiClient.simulateGame(sessionId)
    return {
      sessionId: response.sessionId || '',
      currentGameId: undefined,
      gameIds: [],
      status: response.isCompleted ? 'win' : 'in_progress',
      winner: response.winner as Player,
      moves: response.moves?.map((move) => ({
        player: move.player as Player,
        position: (move.row || 0) * 3 + (move.column || 0),
        timestamp: new Date().toISOString(),
        gameId: response.sessionId
      })) || []
    }
  }

  static async getSession(sessionId: string): Promise<GameSession> {
    // Only run on client side
    if (typeof window === 'undefined') {
      throw new Error('getSession can only be called on the client side');
    }
    
    const response = await apiClient.getSession(sessionId)
    
    console.log(`[ApiService] Response status: 200`)
    console.log(`[ApiService] Session data:`, response)
    
    // Type assertion to access strategy property that will be available after backend update
    const responseWithStrategy = response as ExtendedGetSessionResponse
    
    return {
      sessionId: response.sessionId || '',
      currentGameId: response.gameId,
      gameIds: response.gameId ? [response.gameId] : [],
      status: (response.status as GameStatus) || 'waiting',
      strategy: responseWithStrategy.strategy || 'Random', // Use strategy from backend, fallback to Random
      moves: response.moves?.map((move) => ({
        player: move.player as Player,
        position: (move.row || 0) * 3 + (move.column || 0),
        timestamp: new Date().toISOString(),
        gameId: response.gameId
      })) || []
    }
  }

  static async getStrategies(): Promise<ListStrategiesResponse> {
    // Only run on client side
    if (typeof window === 'undefined') {
      throw new Error('getStrategies can only be called on the client side');
    }
    
    // The strategies endpoint is not in our generated client, so we need to fetch it directly
    // but through our proxy
    const response = await fetch('/api/game/sessions/strategies', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })

    if (!response.ok) {
      throw new Error(`Failed to fetch strategies: ${response.status}`)
    }

    return response.json()
  }
} 