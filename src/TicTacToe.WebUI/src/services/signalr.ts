import { GameState, Move } from '@/types/game'

const SIGNALR_HUB_URL = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL || 'http://localhost:5001/gameHub'

export class SignalRService {
  private connection: any = null
  private isConnected = false

  constructor() {
    // Only initialize on client side
    if (typeof window !== 'undefined') {
      this.initializeConnection()
    }
  }

  private async initializeConnection() {
    try {
      const signalR = await import('@microsoft/signalr')
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(SIGNALR_HUB_URL)
        .withAutomaticReconnect()
        .build()
    } catch (error) {
      console.error('Failed to initialize SignalR:', error)
    }
  }

  async start(): Promise<void> {
    if (!this.connection || typeof window === 'undefined') return

    try {
      await this.connection.start()
      this.isConnected = true
      console.log('SignalR Connected')
    } catch (error) {
      console.error('SignalR Connection Error:', error)
      throw error
    }
  }

  async stop(): Promise<void> {
    if (this.connection && typeof window !== 'undefined') {
      await this.connection.stop()
      this.isConnected = false
    }
  }

  async joinGameSession(sessionId: string): Promise<void> {
    if (!this.connection || !this.isConnected || typeof window === 'undefined') {
      throw new Error('SignalR connection not established')
    }

    await this.connection.invoke('JoinGameSession', sessionId)
  }

  onGameStateUpdated(callback: (gameState: GameState) => void): void {
    if (!this.connection || typeof window === 'undefined') return

    this.connection.on('GameStateUpdated', callback)
  }

  onMoveReceived(callback: (move: Move) => void): void {
    if (!this.connection || typeof window === 'undefined') return

    this.connection.on('MoveReceived', callback)
  }

  onGameCompleted(callback: (finalState: GameState) => void): void {
    if (!this.connection || typeof window === 'undefined') return

    this.connection.on('GameCompleted', callback)
  }

  onError(callback: (errorMessage: string) => void): void {
    if (!this.connection || typeof window === 'undefined') return

    this.connection.on('Error', callback)
  }

  getConnectionState(): boolean {
    return this.isConnected
  }
} 