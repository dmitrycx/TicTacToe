"use client"

import React, { useState, useEffect, useMemo, useRef } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { 
  Play, 
  Square, 
  X, 
  Circle, 
  AlertCircle, 
  Loader2, 
  Zap, 
  Brain, 
  Shuffle, 
  Target, 
  Cpu 
} from 'lucide-react'
import { Select, SelectContent, SelectItem, SelectTrigger } from '@/components/ui/select'
import { Label } from '@/components/ui/label'
import { GameState, GameStrategy, Move, GameSession } from '@/types/game'
import { ApiService, StrategyInfo } from '@/services/api'
import { getSignalRService } from '@/services/signalr'

export default function TicTacToeGame() {
  const [gameState, setGameState] = useState<GameState>({
    board: Array(9).fill(null),
    status: "waiting",
  })
  const [session, setSession] = useState<GameSession | null>(null)
  const [isConnected, setIsConnected] = useState(false)
  const [isSimulating, setIsSimulating] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [moveHistory, setMoveHistory] = useState<Move[]>([])
  const [selectedStrategy, setSelectedStrategy] = useState<GameStrategy>("Random")
  const [winningSquares, setWinningSquares] = useState<number[]>([])
  const [currentGameId, setCurrentGameId] = useState<string | null>(null)
  
  // Use refs to access current values in SignalR event handlers
  const sessionRef = useRef<GameSession | null>(null)
  const currentGameIdRef = useRef<string | null>(null)
  
  // Update refs when state changes
  useEffect(() => {
    sessionRef.current = session
  }, [session])
  
  useEffect(() => {
    currentGameIdRef.current = currentGameId
  }, [currentGameId])
  
  // NEW: Add state to hold the strategies fetched from the API
  const [availableStrategies, setAvailableStrategies] = useState<StrategyInfo[]>([])

  // NEW: Fetch strategies when the component mounts
  useEffect(() => {
    const fetchStrategies = async () => {
      try {
        const response = await ApiService.getStrategies()
        setAvailableStrategies(response.strategies)
        
        // Set the first available strategy as default if we have strategies
        if (response.strategies.length > 0) {
          setSelectedStrategy(response.strategies[0].name as GameStrategy)
        }
      } catch (error) {
        console.error("Failed to fetch strategies", error)
        // Fallback to a default list if the API call fails
        setAvailableStrategies([
          { 
            name: "Random", 
            displayName: "Random",
            description: "Makes random moves"
          }
        ])
      }
    }
    fetchStrategies()
  }, []) // Empty dependency array means this runs once on mount

  // NEW: Use useMemo to map the fetched data to the UI structure with icons
  const GAME_STRATEGIES_FOR_UI = useMemo(() => {
    // Frontend handles UI concerns (icons and colors)
    const iconMap: Record<string, React.ReactNode> = {
      Random: <Shuffle className="w-4 h-4" />,
      RuleBased: <Target className="w-4 h-4" />,
      AI: <Brain className="w-4 h-4" />,
      MinMax: <Zap className="w-4 h-4" />,
      AlphaBeta: <Cpu className="w-4 h-4" />,
    }
    
    const colorMap: Record<string, string> = {
      Random: "text-slate-600",
      RuleBased: "text-blue-600", 
      AI: "text-purple-600",
      MinMax: "text-emerald-600",
      AlphaBeta: "text-red-600",
    }

    return availableStrategies.map(strategy => ({
      value: strategy.name as GameStrategy,
      label: strategy.displayName,
      description: strategy.description,
      icon: iconMap[strategy.name] || <Cpu className="w-4 h-4" />, // Default icon
      color: colorMap[strategy.name] || "text-slate-600",
    }))
  }, [availableStrategies]) // This will re-run only when the fetched strategies change

  // Start SignalR connection
  useEffect(() => {
    if (typeof window === 'undefined') return

    const initializeSignalR = async () => {
      try {
        const signalRService = getSignalRService()
        
        // Wrap the start call in a try-catch to prevent Next.js error boundary from catching SignalR errors
        try {
          await signalRService.start()
        } catch (signalRError) {
          // If this is a WebSocket error in development, just log it and continue
          if (process.env.NODE_ENV === 'development' && signalRError instanceof Error) {
            const errorMessage = signalRError.message.toLowerCase();
            if (errorMessage.includes('websocket') || 
                errorMessage.includes('failed to start the transport') ||
                errorMessage.includes('connection could not be found')) {
              console.log('[SignalR] Suppressed WebSocket error in development mode:', signalRError.message);
              // Don't set error state for expected WebSocket failures
              return;
            }
          }
          // Re-throw non-WebSocket errors
          throw signalRError;
        }
        
        setIsConnected(true)
        setError(null)

        // Set up event handlers
        signalRService.onGameStateUpdated((updatedGameState: GameState) => {
          // Set current game ID if not set
          if (updatedGameState.gameId && !currentGameIdRef.current) {
            setCurrentGameId(updatedGameState.gameId)
          }
          
          // Only update if this is for the current game
          if (!currentGameIdRef.current || updatedGameState.gameId === currentGameIdRef.current) {
            setGameState(updatedGameState)
          }
        })

        signalRService.onMoveReceived((move: Move) => {
          // Set current game ID if not set
          if (move.gameId && !currentGameIdRef.current) {
            setCurrentGameId(move.gameId)
          }
          
          // Only add moves for the current game
          if (!currentGameIdRef.current || move.gameId === currentGameIdRef.current) {
            setMoveHistory((prev) => [...prev, move])
          }
        })

        signalRService.onGameCompleted((finalState: GameState) => {
          // Set current game ID if not set
          if (finalState.gameId && !currentGameIdRef.current) {
            setCurrentGameId(finalState.gameId)
          }
          
          // Only update if this is for the current game
          if (!currentGameIdRef.current || finalState.gameId === currentGameIdRef.current) {
            setGameState(finalState)
            setIsSimulating(false)
            
            // Refresh session data to get the final GameId and other updated information
            if (sessionRef.current?.sessionId) {
              refreshSession(sessionRef.current.sessionId)
            }
          }
        })

        signalRService.onError((errorMessage: string) => {
          // Only show errors that aren't related to WebSocket fallback
          if (!errorMessage.toLowerCase().includes('websocket') && 
              !errorMessage.toLowerCase().includes('connection could not be found')) {
            setError(errorMessage)
          }
          setIsSimulating(false)
        })
      } catch (error) {
        console.error('SignalR initialization error:', error)
        // Only show connection errors that aren't expected WebSocket fallbacks
        if (error instanceof Error) {
          const errorMessage = error.message.toLowerCase();
          if (!errorMessage.includes('websocket failed to connect') &&
              !errorMessage.includes('the connection could not be found on the server') &&
              !errorMessage.includes('failed to start the transport')) {
            setError('Failed to connect to game server')
          }
        } else {
          setError('Failed to connect to game server')
        }
      }
    }

    initializeSignalR()

    return () => {
      // Cleanup will be handled by the singleton
    }
  }, []) // Empty dependency array - only run once on mount

  const createSession = async (strategy: GameStrategy): Promise<string | null> => {
    try {
      const sessionData = await ApiService.createSession({ strategy })
      setSession(sessionData)
      return sessionData.sessionId
    } catch {
      setError('Failed to create game session')
      return null
    }
  }

  const refreshSession = async (sessionId: string) => {
    try {
      const sessionData = await ApiService.getSession(sessionId)
      setSession(sessionData)
    } catch (error) {
      console.error('Failed to refresh session data:', error)
      // Don't throw the error - just log it and continue
      // The session data will be updated when the user starts a new simulation
    }
  }

  const startSimulation = async () => {
    if (!isConnected) {
      setError('Not connected to game server')
      return
    }

    setIsSimulating(true)
    setError(null)
    setMoveHistory([])
    setWinningSquares([])
    setGameState({
      board: Array(9).fill(null),
      status: "in_progress",
    })

    try {
      const signalRService = getSignalRService()
      
      // Create session only if none exists
      let sessionId: string
      if (!session) {
        const newSessionId = await createSession(selectedStrategy)
        if (!newSessionId) {
          setIsSimulating(false)
          return
        }
        sessionId = newSessionId
      } else {
        sessionId = session.sessionId
      }

      // Set current game ID to null initially - it will be updated when we get the first notification
      setCurrentGameId(null)

      await signalRService.joinGameSession(sessionId)
      await ApiService.simulateGame(sessionId)
      
      // Refresh session data to get the updated GameId
      setTimeout(() => refreshSession(sessionId), 1000)
    } catch {
      setError('Failed to start game simulation')
      setIsSimulating(false)
    }
  }

  const renderSquare = (index: number) => {
    const value = gameState.board[index]
    const isWinning = winningSquares.includes(index)

    return (
      <div 
        key={index} 
        className={`relative w-12 h-12 sm:w-16 sm:h-16 border-2 border-slate-200 bg-white/80 backdrop-blur-sm
                   flex items-center justify-center text-2xl sm:text-3xl font-bold
                   transition-all duration-300 ease-out transform-gpu
                   hover:bg-white hover:border-slate-300 hover:-translate-y-0.5
                   active:scale-95
                   ${isWinning ? 'bg-gradient-to-br from-emerald-50 to-emerald-100 border-emerald-300 shadow-glow-emerald' : ''}`}
      >
        {value === "X" && (
          <X className="w-6 h-6 sm:w-10 sm:h-10 text-blue-600 animate-scale-in transition-all duration-200 hover:text-blue-700" />
        )}
        {value === "O" && (
          <Circle className="w-6 h-6 sm:w-10 sm:h-10 text-red-600 animate-scale-in transition-all duration-200 hover:text-red-700" />
        )}
        {value === null && (
          <Square className="w-6 h-6 sm:w-10 sm:h-10 text-slate-200 opacity-0 group-hover:opacity-30 transition-opacity duration-200" />
        )}
      </div>
    )
  }

  const getStatusMessage = () => {
    switch (gameState.status) {
      case "waiting":
        return "Ready to start simulation"
      case "in_progress":
        return `Game in progress - ${gameState.currentPlayer || "X"}'s turn`
      case "win":
        return `Game Over - ${gameState.winner} wins!`
      case "draw":
        return "Game Over - It's a draw!"
      default:
        return "Unknown status"
    }
  }

  const getStatusBadgeStyle = () => {
    switch (gameState.status) {
      case "waiting":
        return "bg-slate-100 text-slate-700 border-slate-200"
      case "in_progress":
        return "bg-blue-100 text-blue-700 border-blue-200 animate-pulse-slow"
      case "win":
        return "bg-emerald-100 text-emerald-700 border-emerald-200"
      case "draw":
        return "bg-amber-100 text-amber-700 border-amber-200"
      default:
        return "bg-slate-100 text-slate-700 border-slate-200"
    }
  }

  const selectedStrategyData = GAME_STRATEGIES_FOR_UI.find((s) => s.value === selectedStrategy)

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-indigo-50/50">
      <div className="relative z-10 p-2 sm:p-3 lg:p-4 min-h-screen flex flex-col">
        <div className="max-w-7xl mx-auto flex-1 flex flex-col">
          {/* Header */}
          <div className="text-center mb-3 sm:mb-4 animate-fade-in">
            <div className="inline-flex items-center gap-1 sm:gap-2 mb-1 sm:mb-2">
              <div className="p-1.5 sm:p-2 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl shadow-large">
                <Brain className="w-4 h-4 sm:w-6 sm:h-6 text-white" />
              </div>
              <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold bg-gradient-to-r from-slate-900 via-blue-800 to-indigo-800 bg-clip-text text-transparent" data-testid="main-header">
                Automated Tic Tac Toe
              </h1>
            </div>
            <p className="text-sm sm:text-base text-slate-600 max-w-2xl mx-auto leading-relaxed px-2">
              Watch as intelligent microservices compete in strategic Tic Tac Toe battles
            </p>
          </div>

          {/* Main Content Grid - Responsive Layout */}
          <div className="grid grid-cols-1 lg:grid-cols-3 xl:grid-cols-4 gap-3 sm:gap-4 flex-1 min-h-0">
            {/* Game Board Section */}
            <div className="lg:col-span-2 xl:col-span-3">
              <Card className="p-3 sm:p-4 bg-gradient-to-br from-white/90 to-slate-50/90 backdrop-blur-sm border border-slate-200/50 rounded-xl shadow-soft transition-all duration-300 ease-out hover:shadow-medium h-full flex flex-col">
                <CardHeader className="pb-2 sm:pb-3">
                  <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2 sm:gap-0">
                    <CardTitle className="text-lg sm:text-xl font-bold text-slate-800 flex items-center gap-2" data-testid="game-arena-title">
                      <div className="p-1 sm:p-1.5 bg-gradient-to-br from-blue-100 to-indigo-100 rounded-lg">
                        <Target className="w-4 h-4 sm:w-5 sm:h-5 text-blue-600" />
                      </div>
                      Game Arena
                    </CardTitle>
                    <div className={`px-3 sm:px-4 py-1.5 sm:py-2 rounded-full text-xs sm:text-sm font-semibold tracking-wide transition-all duration-300 ease-out border ${getStatusBadgeStyle()}`} data-testid="game-status-badge">
                      {getStatusMessage()}
                    </div>
                  </div>
                </CardHeader>
                <CardContent className="space-y-3 sm:space-y-4 flex-1 flex flex-col">
                  {/* Strategy Selection */}
                  <div className="space-y-2">
                    <Label
                      htmlFor="strategy-select"
                      className="text-sm sm:text-base font-semibold text-slate-700 flex items-center gap-2"
                      data-testid="strategy-label"
                    >
                      <Zap className="w-3 h-3 sm:w-4 sm:h-4 text-amber-500" />
                      Game Strategy
                    </Label>
                    <Select
                      value={selectedStrategy}
                      onValueChange={(value: GameStrategy) => setSelectedStrategy(value)}
                      disabled={isSimulating}
                      data-testid="strategy-select"
                    >
                      <SelectTrigger
                        id="strategy-select"
                        className="h-9 sm:h-10 bg-white/80 backdrop-blur-sm border-slate-200/50 shadow-soft hover:shadow-medium transition-all duration-300"
                      >
                        <div className="flex items-center gap-2">
                          {selectedStrategyData && (
                            <>
                              <div className={`p-1 rounded-lg bg-slate-50 ${selectedStrategyData.color}`}>
                                {selectedStrategyData.icon}
                              </div>
                              <span className="font-medium text-slate-800 text-sm sm:text-base">{selectedStrategyData.label}</span>
                            </>
                          )}
                          {!selectedStrategyData && (
                            <span className="text-slate-500 text-sm sm:text-base">Select a strategy</span>
                          )}
                        </div>
                      </SelectTrigger>
                      <SelectContent className="bg-white/95 backdrop-blur-md border-slate-200/50">
                        {GAME_STRATEGIES_FOR_UI.map((strategy) => (
                          <SelectItem key={strategy.value} value={strategy.value} className="py-2 sm:py-3">
                            <div className="flex items-center gap-2">
                              <div className={`p-1 sm:p-1.5 rounded-lg bg-slate-50 ${strategy.color}`}>{strategy.icon}</div>
                              <div className="flex flex-col">
                                <span className="font-semibold text-slate-800 text-sm sm:text-base">{strategy.label}</span>
                                <span className="text-xs text-slate-500">{strategy.description}</span>
                              </div>
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Connection Status - Subtle indicator */}
                  <div className="flex items-center gap-2 px-2 sm:px-3 py-1.5 sm:py-2 rounded-lg bg-white/40 backdrop-blur-sm border border-slate-200/30 shadow-soft opacity-80" data-testid="connection-status">
                    <div
                      className={`w-2 h-2 sm:w-3 sm:h-3 rounded-full transition-all duration-300 ${
                        isConnected
                          ? "bg-emerald-500 shadow-glow-emerald animate-pulse-slow"
                          : "bg-slate-400"
                      }`}
                    />
                    <span className="text-xs sm:text-sm font-medium text-slate-600">
                      {isConnected ? "Live connection" : "Connecting..."}
                    </span>
                    {isConnected && (
                      <Badge variant="outline" className="bg-emerald-50/80 text-emerald-600 border-emerald-200/50 text-xs">
                        Ready
                      </Badge>
                    )}
                  </div>

                  {/* Error Display */}
                  {error && (
                    <Alert
                      variant="destructive"
                      className="bg-red-50/80 backdrop-blur-sm border-red-200/50 animate-slide-down"
                      data-testid="error-alert"
                    >
                      <AlertCircle className="h-4 w-4 sm:h-5 sm:w-5" />
                      <AlertDescription className="font-medium text-sm sm:text-base">{error}</AlertDescription>
                    </Alert>
                  )}

                  {/* Game Board */}
                  <div className="flex justify-center flex-1 items-center">
                    <div className="p-2 sm:p-4 bg-gradient-to-br from-white/90 to-slate-50/90 backdrop-blur-sm rounded-2xl shadow-large border border-slate-200/50">
                      <div className="grid grid-cols-3 gap-0.5 sm:gap-1" data-testid="game-board">
                        {Array.from({ length: 9 }, (_, index) => (
                          <div key={index} data-testid={`board-cell-${index}`}>{renderSquare(index)}</div>
                        ))}
                      </div>
                    </div>
                  </div>

                  {/* Controls */}
                  <div className="flex justify-center">
                    <Button
                      onClick={startSimulation}
                      disabled={!isConnected || isSimulating}
                      size="lg"
                      className="min-w-[160px] h-9 sm:h-10 px-4 sm:px-6 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 
               text-white font-semibold rounded-xl shadow-large text-sm sm:text-base
               transition-all duration-300 ease-out transform-gpu
               hover:brightness-110 hover:-translate-y-0.5
               active:scale-95 active:brightness-90
               disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0 disabled:hover:brightness-100"
                      data-testid="start-battle-btn"
                    >
                      {isSimulating ? (
                        <>
                          <Loader2 className="w-3 h-3 sm:w-4 sm:h-4 mr-1.5 sm:mr-2 animate-spin" data-testid="loading-indicator" />
                          Simulating...
                        </>
                      ) : (
                        <>
                          <Play className="w-3 h-3 sm:w-4 sm:h-4 mr-1.5 sm:mr-2" />
                          Start Battle
                        </>
                      )}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Move History Section */}
            <div className="lg:col-span-1 xl:col-span-1">
              <Card className="p-3 sm:p-4 bg-gradient-to-br from-white/90 to-slate-50/90 backdrop-blur-sm border border-slate-200/50 rounded-xl shadow-soft transition-all duration-300 ease-out h-full flex flex-col">
                <CardHeader className="pb-2 sm:pb-3">
                  <CardTitle className="text-base sm:text-lg font-bold text-slate-800 flex items-center gap-2">
                    <div className="p-1 sm:p-1.5 bg-gradient-to-br from-amber-100 to-orange-100 rounded-lg">
                      <Circle className="w-3 h-3 sm:w-4 sm:h-4 text-amber-600" />
                    </div>
                    Battle Log
                  </CardTitle>
                </CardHeader>
                <CardContent className="flex-1 min-h-0">
                  <div className="space-y-1.5 sm:space-y-2 h-full overflow-y-auto custom-scrollbar">
                    {moveHistory.length === 0 ? (
                      <div className="text-center py-6 sm:py-8">
                        <div className="p-2 sm:p-3 bg-slate-100 rounded-full w-fit mx-auto mb-2 sm:mb-3">
                          <Square className="w-4 h-4 sm:w-6 sm:h-6 text-slate-400" />
                        </div>
                        <p className="text-slate-500 font-medium text-xs sm:text-sm">No moves yet</p>
                        <p className="text-xs text-slate-400 mt-1">Start a battle to see the action</p>
                      </div>
                    ) : (
                      moveHistory.map((move, index) => (
                        <div key={index} className="flex items-center justify-between p-1.5 sm:p-2 bg-gradient-to-r from-white/80 to-slate-50/80 backdrop-blur-sm border border-slate-200/50 rounded-lg shadow-soft transition-all duration-300 ease-out hover:bg-white/90 hover:-translate-y-px animate-slide-up transform-gpu">
                          <div className="flex items-center gap-2 sm:gap-3">
                            <Badge
                              variant="outline"
                              className="bg-slate-50 text-slate-600 border-slate-200 font-mono text-xs"
                            >
                              #{index + 1}
                            </Badge>
                            <div className="flex items-center gap-1.5 sm:gap-2">
                              <div
                                className={`p-1 sm:p-1.5 rounded-lg ${
                                  move.player === "X" ? "bg-blue-100 text-blue-600" : "bg-red-100 text-red-600"
                                }`}
                              >
                                {move.player === "X" ? <X className="w-2.5 h-2.5 sm:w-3 sm:h-3" /> : <Circle className="w-2.5 h-2.5 sm:w-3 sm:h-3" />}
                              </div>
                              <div>
                                <span className="font-semibold text-slate-800 text-xs sm:text-sm">Player {move.player}</span>
                                <p className="text-xs text-slate-500">Position {move.position + 1}</p>
                              </div>
                            </div>
                          </div>
                          <span className="text-xs text-slate-400 font-mono">
                            {new Date(move.timestamp).toLocaleTimeString()}
                          </span>
                        </div>
                      ))
                    )}
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>

          {/* Session Info - Always visible and scrollable */}
          {session && (
            <Card className="mt-3 sm:mt-4 p-3 sm:p-4 bg-gradient-to-br from-white/90 to-slate-50/90 backdrop-blur-sm border border-slate-200/50 rounded-xl shadow-soft transition-all duration-300 ease-out animate-fade-in">
              <CardHeader className="pb-1 sm:pb-2">
                <CardTitle className="text-base sm:text-lg font-bold text-slate-800 flex items-center gap-2">
                  <div className="p-1 sm:p-1.5 bg-gradient-to-br from-purple-100 to-pink-100 rounded-lg">
                    <Cpu className="w-3 h-3 sm:w-4 sm:h-4 text-purple-600" />
                  </div>
                  Session Intelligence
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-2 sm:gap-3">
                  <div className="p-2 sm:p-3 bg-gradient-to-br from-blue-50 to-indigo-50 rounded-lg border border-blue-100">
                    <span className="text-xs font-semibold text-blue-600 uppercase tracking-wide">Session ID</span>
                    <p className="font-mono text-xs text-slate-700 mt-1 break-all">{session.sessionId}</p>
                  </div>
                  <div className="p-2 sm:p-3 bg-gradient-to-br from-emerald-50 to-teal-50 rounded-lg border border-emerald-100">
                    <span className="text-xs font-semibold text-emerald-600 uppercase tracking-wide">Current Game ID</span>
                    <p className="font-mono text-xs text-slate-700 mt-1 break-all">
                      {session.currentGameId || "Not yet created"}
                    </p>
                  </div>
                  <div className="p-2 sm:p-3 bg-gradient-to-br from-purple-50 to-pink-50 rounded-lg border border-purple-100">
                    <span className="text-xs font-semibold text-purple-600 uppercase tracking-wide">Games Played</span>
                    <p className="font-mono text-xs text-slate-700 mt-1">
                      {session.gameIds.length} games
                    </p>
                  </div>
                  <div className="p-2 sm:p-3 bg-gradient-to-br from-amber-50 to-orange-50 rounded-lg border border-amber-100">
                    <span className="text-xs font-semibold text-amber-600 uppercase tracking-wide">Strategy</span>
                    <div className="flex items-center gap-1 mt-1">
                      {selectedStrategyData && (
                        <>
                          <div className={`p-0.5 rounded ${selectedStrategyData.color}`}>
                            {selectedStrategyData.icon}
                          </div>
                          <Badge variant="outline" className="bg-white/80 text-slate-700 border-slate-200 text-xs">
                            {session.strategy}
                          </Badge>
                        </>
                      )}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </div>
  )
}