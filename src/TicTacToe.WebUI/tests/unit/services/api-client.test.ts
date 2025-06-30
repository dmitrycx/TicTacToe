import { ApiClient } from '@/services/api-client'

// Mock the generated client
jest.mock('@/services/generated-client', () => ({
  GameSessionClient: jest.fn().mockImplementation(() => ({
    ticTacToeGameSessionEndpointsCreateSessionEndpoint: jest.fn().mockResolvedValue({}),
    ticTacToeGameSessionEndpointsListSessionsEndpoint: jest.fn().mockResolvedValue([]),
    ticTacToeGameSessionEndpointsGetSessionEndpoint: jest.fn().mockResolvedValue({}),
    ticTacToeGameSessionEndpointsDeleteSessionEndpoint: jest.fn().mockResolvedValue({}),
    ticTacToeGameSessionEndpointsSimulateGameEndpoint: jest.fn().mockResolvedValue({}),
  })),
  TicTacToeGameSessionEndpointsSimulateGameRequest: jest.fn().mockImplementation(() => ({
    moveStrategy: undefined,
  })),
}))

describe('API Client', () => {
  let apiClient: ApiClient

  beforeEach(() => {
    // Reset the singleton instance
    (ApiClient as any).instance = undefined
    apiClient = ApiClient.getInstance()
  })

  it('can list sessions', async () => {
    const result = await apiClient.listSessions()
    expect(result).toBeDefined()
  })

  it('can get a session by id', async () => {
    const result = await apiClient.getSession('test-id')
    expect(result).toBeDefined()
  })

  it('can delete a session', async () => {
    const result = await apiClient.deleteSession('test-id')
    expect(result).toBeDefined()
  })

  it('can simulate a game', async () => {
    const result = await apiClient.simulateGame('test-id')
    expect(result).toBeDefined()
  })

  it('returns the same instance when getInstance is called multiple times', () => {
    const instance1 = ApiClient.getInstance()
    const instance2 = ApiClient.getInstance()
    expect(instance1).toBe(instance2)
  })

  it('handles simulateGame with strategy parameter', async () => {
    const result = await apiClient.simulateGame('test-id', 'RuleBased')
    expect(result).toBeDefined()
  })

  it('handles invalid session ID', async () => {
    // Mock the client to throw an error for invalid session ID
    const mockClient = {
      ticTacToeGameSessionEndpointsGetSessionEndpoint: jest.fn().mockRejectedValue(new Error('Session not found')),
    }
    
    // Temporarily replace the client
    const originalClient = (apiClient as any).gameSessionClient
    ;(apiClient as any).gameSessionClient = mockClient
    
    await expect(apiClient.getSession('invalid-id')).rejects.toThrow('Session not found')
    
    // Restore the original client
    ;(apiClient as any).gameSessionClient = originalClient
  })

  it('handles empty response from API', async () => {
    // Mock the client to return empty response
    const mockClient = {
      ticTacToeGameSessionEndpointsListSessionsEndpoint: jest.fn().mockResolvedValue(null),
    }
    
    // Temporarily replace the client
    const originalClient = (apiClient as any).gameSessionClient
    ;(apiClient as any).gameSessionClient = mockClient
    
    const result = await apiClient.listSessions()
    expect(result).toBeNull()
    
    // Restore the original client
    ;(apiClient as any).gameSessionClient = originalClient
  })

  it('handles simulation errors', async () => {
    // Mock the client to throw an error during simulation
    const mockClient = {
      ticTacToeGameSessionEndpointsSimulateGameEndpoint: jest.fn().mockRejectedValue(new Error('Simulation failed')),
    }
    
    // Temporarily replace the client
    const originalClient = (apiClient as any).gameSessionClient
    ;(apiClient as any).gameSessionClient = mockClient
    
    await expect(apiClient.simulateGame('test-id')).rejects.toThrow('Simulation failed')
    
    // Restore the original client
    ;(apiClient as any).gameSessionClient = originalClient
  })

  it('handles delete session errors', async () => {
    // Mock the client to throw an error during deletion
    const mockClient = {
      ticTacToeGameSessionEndpointsDeleteSessionEndpoint: jest.fn().mockRejectedValue(new Error('Delete failed')),
    }
    
    // Temporarily replace the client
    const originalClient = (apiClient as any).gameSessionClient
    ;(apiClient as any).gameSessionClient = mockClient
    
    await expect(apiClient.deleteSession('test-id')).rejects.toThrow('Delete failed')
    
    // Restore the original client
    ;(apiClient as any).gameSessionClient = originalClient
  })
}) 