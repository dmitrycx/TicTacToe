import { render, screen } from '@testing-library/react'
import TicTacToeGame from '@/components/TicTacToeGame'

// Mock the API client
jest.mock('@/services/api-client', () => ({
  apiClient: {
    listSessions: jest.fn(),
    createSession: jest.fn(),
    simulateGame: jest.fn(),
  },
}))

// Mock SignalR service
jest.mock('@/services/signalr', () => ({
  useSignalRConnection: jest.fn(() => ({
    connection: {
      state: 'Connected',
      on: jest.fn(),
      off: jest.fn(),
    },
    isConnected: true,
  })),
  getSignalRService: jest.fn(() => ({
    start: jest.fn().mockResolvedValue(undefined),
    joinGameSession: jest.fn().mockResolvedValue(undefined),
    onGameStateUpdated: jest.fn(),
    onMoveReceived: jest.fn(),
    onGameCompleted: jest.fn(),
    onError: jest.fn(),
  })),
}))

describe('TicTacToeGame', () => {
  it('renders the main header', () => {
    render(<TicTacToeGame />)
    expect(screen.getByTestId('main-header')).toBeInTheDocument()
  })

  it('renders the game arena title', () => {
    render(<TicTacToeGame />)
    expect(screen.getByTestId('game-arena-title')).toBeInTheDocument()
  })

  it('renders the game board', () => {
    render(<TicTacToeGame />)
    expect(screen.getByTestId('game-board')).toBeInTheDocument()
  })

  it('renders the start battle button', () => {
    render(<TicTacToeGame />)
    expect(screen.getByTestId('start-battle-btn')).toBeInTheDocument()
  })

  it('renders the game status badge', () => {
    render(<TicTacToeGame />)
    expect(screen.getByTestId('game-status-badge')).toBeInTheDocument()
  })

  it('renders the connection status', () => {
    render(<TicTacToeGame />)
    expect(screen.getByTestId('connection-status')).toBeInTheDocument()
  })
}) 