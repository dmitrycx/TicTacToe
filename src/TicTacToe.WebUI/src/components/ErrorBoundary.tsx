'use client'

import React, { Component, ErrorInfo, ReactNode } from 'react'

interface Props {
  children: ReactNode
  fallback?: ReactNode
}

interface State {
  hasError: boolean
  error?: Error
}

class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props)
    this.state = { hasError: false }
  }

  static getDerivedStateFromError(error: Error): State {
    // Check if this is a SignalR WebSocket error that we want to suppress
    if (process.env.NODE_ENV === 'development') {
      const errorMessage = error.message.toLowerCase()
      if (errorMessage.includes('websocket failed to connect') ||
          errorMessage.includes('failed to start the transport') ||
          errorMessage.includes('connection could not be found on the server')) {
        console.log('[ErrorBoundary] Suppressed SignalR WebSocket error:', error.message)
        // Don't set error state for expected WebSocket failures
        return { hasError: false }
      }
    }
    
    return { hasError: true, error }
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    // Check if this is a SignalR WebSocket error that we want to suppress
    if (process.env.NODE_ENV === 'development') {
      const errorMessage = error.message.toLowerCase()
      if (errorMessage.includes('websocket failed to connect') ||
          errorMessage.includes('failed to start the transport') ||
          errorMessage.includes('connection could not be found on the server')) {
        console.log('[ErrorBoundary] Suppressed SignalR WebSocket error:', error.message)
        // Don't log to console for expected WebSocket failures
        return
      }
    }
    
    console.error('[ErrorBoundary] Caught error:', error, errorInfo)
  }

  render() {
    if (this.state.hasError) {
      // You can render any custom fallback UI
      return this.props.fallback || (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
          <h2 className="text-red-800 font-semibold">Something went wrong</h2>
          <p className="text-red-600 text-sm mt-1">
            {this.state.error?.message || 'An unexpected error occurred'}
          </p>
        </div>
      )
    }

    return this.props.children
  }
}

export default ErrorBoundary 