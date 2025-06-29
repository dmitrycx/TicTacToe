import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import TicTacToeGame from '@/components/TicTacToeGame'
import { ApiService } from '@/services/api'
import { getSignalRService } from '@/services/signalr'

// Mock the API service
jest.mock('@/services/api', () => ({
  ApiService: {
    getStrategies: jest.fn(),
    createSession: jest.fn(),
    getSession: jest.fn(),
    simulateGame: jest.fn(),
  },
}))

// Mock SignalR service
jest.mock('@/services/signalr', () => ({
  getSignalRService: jest.fn(() => ({
    start: jest.fn().mockResolvedValue(undefined),
    joinGameSession: jest.fn().mockResolvedValue(undefined),
    onGameStateUpdated: jest.fn(),
    onMoveReceived: jest.fn(),
    onGameCompleted: jest.fn(),
    onError: jest.fn(),
    getConnectionState: jest.fn().mockReturnValue(true),
  })),
}))

// Mock the window object for SignalR
Object.defineProperty(window, 'GAME_SESSION_SERVICE_URL', {
  value: 'http://localhost:8081',
  writable: true,
})

describe('TicTacToeGame', () => {
  const mockApiService = ApiService as jest.Mocked<typeof ApiService>
  const mockSignalRService = getSignalRService() as jest.Mocked<ReturnType<typeof getSignalRService>>

  beforeEach(() => {
    jest.clearAllMocks()
    
    // Default API responses
    mockApiService.getStrategies.mockResolvedValue({
      strategies: [
        { name: 'Random', displayName: 'Random', description: 'Makes random moves' },
        { name: 'RuleBased', displayName: 'Rule Based', description: 'Uses basic rules' },
      ]
    })
    
    mockApiService.createSession.mockResolvedValue({
      sessionId: 'test-session-id',
      currentGameId: 'test-game-id',
      gameIds: ['test-game-id'],
      status: 'waiting',
      strategy: 'Random',
      moves: []
    })
    
    mockApiService.simulateGame.mockResolvedValue({
      sessionId: 'test-session-id',
      currentGameId: 'test-game-id',
      gameIds: ['test-game-id'],
      status: 'win',
      winner: 'X',
      moves: [
        { player: 'X', position: 0, timestamp: '2024-01-01T00:00:00Z', gameId: 'test-game-id' },
        { player: 'O', position: 4, timestamp: '2024-01-01T00:01:00Z', gameId: 'test-game-id' },
      ]
    })
  })

  it('renders the main header', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('main-header')).toBeInTheDocument()
    })
  })

  it('renders the game arena title', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('game-arena-title')).toBeInTheDocument()
    })
  })

  it('renders the game board', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('game-board')).toBeInTheDocument()
    })
  })

  it('renders the start battle button', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('start-battle-btn')).toBeInTheDocument()
    })
  })

  it('renders the game status badge', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('game-status-badge')).toBeInTheDocument()
    })
  })

  it('renders the connection status', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('connection-status')).toBeInTheDocument()
    })
  })

  it('loads strategies on mount', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(mockApiService.getStrategies).toHaveBeenCalledTimes(1)
    })
  })

  it('creates a session when start battle is clicked', async () => {
    const user = userEvent.setup()
    
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('start-battle-btn')).toBeInTheDocument()
    })
    
    await user.click(screen.getByTestId('start-battle-btn'))
    
    await waitFor(() => {
      expect(mockApiService.createSession).toHaveBeenCalledTimes(1)
    })
  })

  it('shows error when session creation fails', async () => {
    const user = userEvent.setup()
    mockApiService.createSession.mockRejectedValue(new Error('Failed to create session'))
    
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('start-battle-btn')).toBeInTheDocument()
    })
    
    await user.click(screen.getByTestId('start-battle-btn'))
    
    await waitFor(() => {
      expect(screen.getByTestId('error-alert')).toBeInTheDocument()
    })
  })

  it('displays strategy selection dropdown', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByText('Game Strategy')).toBeInTheDocument()
    })
  })

  it('renders all 9 game board cells', async () => {
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      const cells = screen.getAllByTestId(/board-cell-\d+/)
      expect(cells).toHaveLength(9)
    })
  })

  it('shows loading state during simulation', async () => {
    const user = userEvent.setup()
    
    // Mock a delayed response to test loading state
    mockApiService.createSession.mockImplementation(() => 
      new Promise(resolve => setTimeout(() => resolve({
        sessionId: 'test-session-id',
        currentGameId: 'test-game-id',
        gameIds: ['test-game-id'],
        status: 'waiting',
        strategy: 'Random',
        moves: []
      }), 100))
    )
    
    render(<TicTacToeGame />)
    
    await waitFor(() => {
      expect(screen.getByTestId('start-battle-btn')).toBeInTheDocument()
    })
    
    await user.click(screen.getByTestId('start-battle-btn'))
    
    await waitFor(() => {
      expect(screen.getByTestId('loading-indicator')).toBeInTheDocument()
    })
  })
}) 