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
      // Use the proxy route that handles all environments
      const hubUrl = '/api/game/gameHub';
      console.log(`[SignalR] Connecting to proxy: ${hubUrl}`);
      
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
        console.log('[SignalR] Reconnecting... (development mode - WebSocket fallback expected)');
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
      console.log('[SignalR] Connection started successfully via proxy');
    } catch (error) {
      // Suppress specific WebSocket errors in development
      if (process.env.NODE_ENV === 'development' && 
          error instanceof Error && 
          error.message.includes('WebSocket failed to connect')) {
        console.log('[SignalR] WebSocket connection failed (development mode - SSE fallback expected)');
        return; // Don't throw error, let SSE handle it
      }
      
      console.error('[SignalR] Failed to start connection via proxy:', error);
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