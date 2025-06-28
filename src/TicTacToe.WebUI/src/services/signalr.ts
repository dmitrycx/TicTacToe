import { GameState, Move } from '@/types/game'

// Get the service URL from the window object (set by Aspire) or fallback
const getSignalRHubUrl = () => {
  if (typeof window !== 'undefined' && (window as any).GAME_SESSION_SERVICE_URL) {
    const url = `${(window as any).GAME_SESSION_SERVICE_URL}/gameHub`;
    console.log('SignalR: Using window.GAME_SESSION_SERVICE_URL:', (window as any).GAME_SESSION_SERVICE_URL);
    console.log('SignalR: Full hub URL:', url);
    return url;
  }
  // Fallback for when not running under Aspire
  const fallbackUrl = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL || 'http://localhost:8081/gameHub';
  console.log('SignalR: Using fallback URL:', fallbackUrl);
  return fallbackUrl;
};

class SignalRService {
  private connection: import('@microsoft/signalr').HubConnection | null = null
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
      const hubUrl = getSignalRHubUrl();
      console.log('Connecting to SignalR hub:', hubUrl);
      
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
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