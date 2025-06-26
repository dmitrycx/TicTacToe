import { 
  CreateSessionRequest, 
  CreateSessionResponse, 
  SimulateGameResponse,
  GameSession 
} from '@/types/game'

const GAME_SESSION_SERVICE_URL = process.env.NEXT_PUBLIC_GAME_SESSION_SERVICE_URL || 'http://localhost:5001'

export class ApiService {
  static async createSession(request: CreateSessionRequest): Promise<CreateSessionResponse> {
    const response = await fetch(`${GAME_SESSION_SERVICE_URL}/sessions`, {
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
    const response = await fetch(`${GAME_SESSION_SERVICE_URL}/sessions/${sessionId}/simulate`, {
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
    const response = await fetch(`${GAME_SESSION_SERVICE_URL}/sessions/${sessionId}`, {
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
} 