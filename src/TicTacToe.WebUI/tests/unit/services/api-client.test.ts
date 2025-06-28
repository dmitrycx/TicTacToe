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

  it('can create a session', async () => {
    const result = await apiClient.createSession()
    expect(result).toBeDefined()
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
}) 