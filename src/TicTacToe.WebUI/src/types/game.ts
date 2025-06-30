export type GameStatus = "waiting" | "in_progress" | "win" | "draw"
export type Player = "X" | "O" | null
export type Board = Player[]
export type GameStrategy = "Random" | "RuleBased" | "AI" | "MinMax" | "AlphaBeta"

export interface GameState {
  board: Board
  status: GameStatus
  winner?: Player
  currentPlayer?: Player
  gameId?: string
}

export interface Move {
  player: Player
  position: number
  timestamp: string
  gameId?: string
}

export interface GameSession {
  sessionId: string
  currentGameId?: string
  gameIds: string[]
  status: GameStatus
  strategy: GameStrategy
  moves: Move[]
}

export interface CreateSessionRequest {
  strategy: GameStrategy
}

export interface CreateSessionResponse {
  sessionId: string
  currentGameId?: string
  gameIds: string[]
  status: GameStatus
  strategy: GameStrategy
  moves: Move[]
}

export interface SimulateGameResponse {
  sessionId: string
  currentGameId?: string
  gameIds: string[]
  status: GameStatus
  winner?: Player
  moves: Move[]
} 