import '@testing-library/jest-dom'

// Polyfill fetch for Jest environment
global.fetch = jest.fn()

// Mock jest.clearAllMocks for tests that need it
if (typeof jest !== 'undefined') {
  jest.clearAllMocks = jest.fn()
}

// Mock Next.js router
jest.mock('next/navigation', () => ({
  useRouter() {
    return {
      push: jest.fn(),
      replace: jest.fn(),
      prefetch: jest.fn(),
      back: jest.fn(),
      forward: jest.fn(),
      refresh: jest.fn(),
    }
  },
  useSearchParams() {
    return new URLSearchParams()
  },
  usePathname() {
    return '/'
  },
}))

// Mock SignalR
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

// Mock API service
jest.mock('@/services/api', () => ({
  ApiService: {
    getStrategies: jest.fn().mockResolvedValue({
      strategies: [
        { name: 'Random', displayName: 'Random', description: 'Makes random moves' },
        { name: 'RuleBased', displayName: 'Rule Based', description: 'Rule-based moves' },
      ],
    }),
    createSession: jest.fn().mockResolvedValue({ sessionId: '1', status: 'Created', createdAt: '2024-01-01T00:00:00Z', gameIds: [], strategy: 'Random', currentGameId: null }),
    getSession: jest.fn().mockResolvedValue({ sessionId: '1', status: 'InProgress', createdAt: '2024-01-01T00:00:00Z', gameIds: [], strategy: 'Random', currentGameId: null }),
    simulateGame: jest.fn().mockResolvedValue({}),
  },
}))

// Global test utilities
global.ResizeObserver = jest.fn().mockImplementation(() => ({
  observe: jest.fn(),
  unobserve: jest.fn(),
  disconnect: jest.fn(),
}))

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: jest.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: jest.fn(), // deprecated
    removeListener: jest.fn(), // deprecated
    addEventListener: jest.fn(),
    removeEventListener: jest.fn(),
    dispatchEvent: jest.fn(),
  })),
}) 