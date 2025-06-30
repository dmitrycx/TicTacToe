import TicTacToeGame from "@/components/TicTacToeGame"
import ErrorBoundary from "@/components/ErrorBoundary"

export default function Home() {
  return (
    <ErrorBoundary>
      <TicTacToeGame />
    </ErrorBoundary>
  )
}
