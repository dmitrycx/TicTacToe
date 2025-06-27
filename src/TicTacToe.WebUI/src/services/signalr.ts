import { GameState, Move } from '@/types/game'

// Connect directly to the GameSession service's SignalR hub
const SIGNALR_HUB_URL = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL || 'http://localhost:5001/gameHub'

class SignalRService {
  private connection: any = null
  private isInitialized = false
  private static instance: SignalRService | null = null

  private constructor() {}

  static getInstance(): SignalRService {
    if (!SignalRService.instance) {
      SignalRService.instance = new SignalRService()
    }
    return SignalRService.instance
  }

  // Ensures the library is only loaded in the browser
  private async initialize() {
    if (this.isInitialized || typeof window === 'undefined') {
      return
    }
    
    try {
      const signalR = await import('@microsoft/signalr')
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(SIGNALR_HUB_URL)
        .withAutomaticReconnect()
        .build()
      this.isInitialized = true
    } catch (error) {
      console.error('Failed to initialize SignalR:', error)
      throw error
    }
  }

  async start(): Promise<void> {
    await this.initialize()
    if (!this.connection || this.connection.state === 'Connected') return

    try {
      await this.connection.start()
      console.log('SignalR Connected')
    } catch (error) {
      console.error('SignalR Connection Error:', error)
      throw error
    }
  }

  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop()
    }
  }

  async joinGameSession(sessionId: string): Promise<void> {
    if (!this.connection) throw new Error('SignalR connection not established')
    await this.connection.invoke('JoinGameSession', sessionId)
  }

  onGameStateUpdated(callback: (gameState: GameState) => void): void {
    if (!this.connection) return
    this.connection.on('GameStateUpdated', callback)
  }

  onMoveReceived(callback: (move: Move) => void): void {
    if (!this.connection) return
    this.connection.on('MoveReceived', callback)
  }

  onGameCompleted(callback: (finalState: GameState) => void): void {
    if (!this.connection) return
    this.connection.on('GameCompleted', callback)
  }

  onError(callback: (errorMessage: string) => void): void {
    if (!this.connection) return
    this.connection.on('Error', callback)
  }

  getConnectionState(): boolean {
    return !!this.connection && this.connection.state === 'Connected'
  }
}

// Export a function to get the singleton instance
export const getSignalRService = () => SignalRService.getInstance() 