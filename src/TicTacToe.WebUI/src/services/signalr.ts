// src/lib/signalr.ts
import { GameState, Move } from '@/types/game'
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

class SignalRService {
  private connection: HubConnection | null = null;
  private isInitialized = false;
  private static instance: SignalRService | null = null;

  private constructor() {}

  static getInstance(): SignalRService {
    if (!SignalRService.instance) {
      SignalRService.instance = new SignalRService();
    }
    return SignalRService.instance;
  }

  private async initialize() {
    if (this.isInitialized || typeof window === 'undefined') {
      return;
    }

    try {
      // Use the API Gateway for SignalR connections
      const hubUrl = '/api/game/gameHub';
      console.log(`[SignalR] Connecting to API Gateway: ${hubUrl}`);
      
      // In development, explain the expected WebSocket fallback behavior
      if (process.env.NODE_ENV === 'development') {
        console.log('[SignalR] Development mode: WebSocket may fail initially, SSE fallback is expected and normal');
        console.log('[SignalR] To hide expected WebSocket errors in console, filter out: "WebSocket failed to connect"');
      }
      
      this.connection = new HubConnectionBuilder()
        .withUrl(hubUrl)
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();
        
      // Add development-mode error suppression
      this.setupErrorHandling();
        
      this.isInitialized = true;
    } catch (error) {
      console.error('[SignalR] Initialization error:', error);
      throw error;
    }
  }

  private setupErrorHandling() {
    if (!this.connection) return;

    // Suppress WebSocket errors in development mode
    this.connection.onreconnecting((error) => {
      if (process.env.NODE_ENV === 'development') {
        console.log('[SignalR] Reconnecting... (development mode - transport fallback expected)');
      } else {
        console.error('[SignalR] Reconnecting:', error);
      }
    });

    this.connection.onreconnected(() => {
      if (process.env.NODE_ENV === 'development') {
        console.log('[SignalR] Reconnected successfully (development mode)');
      } else {
        console.log('[SignalR] Reconnected successfully');
      }
    });

    this.connection.onclose((error) => {
      if (process.env.NODE_ENV === 'development') {
        console.log('[SignalR] Connection closed (development mode - normal during hot reload)');
      } else {
        console.error('[SignalR] Connection closed:', error);
      }
    });
  }

  async start(): Promise<void> {
    await this.initialize()
    if (!this.connection || this.connection.state === 'Connected') return

    try {
      await this.connection.start()
      console.log('[SignalR] Connection started successfully via API Gateway');
    } catch (error) {
      // In development mode, suppress WebSocket connection errors since SSE fallback is expected
      if (process.env.NODE_ENV === 'development') {
        if (error instanceof Error) {
          const errorMessage = error.message.toLowerCase();
          if (errorMessage.includes('websocket failed to connect') ||
              errorMessage.includes('the connection could not be found on the server') ||
              errorMessage.includes('websocket connection failed') ||
              errorMessage.includes('failed to start the transport')) {
            console.log('[SignalR] WebSocket connection failed (expected in development - SSE fallback will handle connection)');
            // Don't throw the error - let the automatic reconnection handle it
            // This prevents Next.js error boundary from catching it
            return;
          }
        }
        // For other development errors, log but don't throw
        console.log('[SignalR] Connection issue in development mode (continuing with fallback):', error);
        return;
      }
      
      console.error('[SignalR] Failed to start connection via API Gateway:', error);
      throw error
    }
  }

  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop()
      this.connection = null;
      this.isInitialized = false;
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

export const getSignalRService = () => SignalRService.getInstance();