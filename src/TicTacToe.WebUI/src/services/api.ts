import { 
  CreateSessionRequest, 
  CreateSessionResponse, 
  SimulateGameResponse,
  GameSession 
} from '@/types/game'

export interface StrategyInfo {
  name: string;
  displayName: string;
  description: string;
}

export interface ListStrategiesResponse {
  strategies: StrategyInfo[];
}

export class ApiService {
  static async createSession(request: CreateSessionRequest): Promise<CreateSessionResponse> {
    const response = await fetch('/api/sessions', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    if (!response.ok) {
      throw new Error(`Failed to create session: ${response.status}`)
    }

    return response.json()
  }

  static async simulateGame(sessionId: string): Promise<SimulateGameResponse> {
    const response = await fetch(`/api/sessions/${sessionId}/simulate`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
    })

    if (!response.ok) {
      throw new Error(`Failed to simulate game: ${response.status}`)
    }

    return response.json()
  }

  static async getSession(sessionId: string): Promise<GameSession> {
    const response = await fetch(`/api/sessions/${sessionId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })

    if (!response.ok) {
      throw new Error(`Failed to get session: ${response.status}`)
    }

    return response.json()
  }

  static async getStrategies(): Promise<ListStrategiesResponse> {
    const response = await fetch('/api/sessions/strategies', {
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