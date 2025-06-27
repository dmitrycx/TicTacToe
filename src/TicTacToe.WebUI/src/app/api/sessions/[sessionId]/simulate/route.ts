import { NextResponse } from 'next/server'
import { serverApiRequest } from '@/lib/server-api'
import { AxiosError } from 'axios'

/**
 * @swagger
 * /api/sessions/{sessionId}/simulate:
 *   post:
 *     summary: Simulate a game session
 *     description: Acts as a BFF to initiate a game simulation for a specific session, allowing the session to play moves automatically until completion.
 *     parameters:
 *       - in: path
 *         name: sessionId
 *         required: true
 *         schema:
 *           type: string
 *           format: uuid
 *         description: The unique identifier of the session to simulate
 *     requestBody:
 *       required: false
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               moveDelayMs:
 *                 type: number
 *                 description: Delay between moves in milliseconds (optional)
 *               maxMoves:
 *                 type: number
 *                 description: Maximum number of moves to simulate (optional)
 *     responses:
 *       200:
 *         description: Simulation completed successfully.
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/SimulateGameResponse'
 *       400:
 *         description: Invalid simulation request.
 *       404:
 *         description: Session not found.
 *       409:
 *         description: Session is not in a valid state for simulation.
 *       500:
 *         description: Internal server error.
 */
export async function POST(
  request: Request,
  { params }: { params: Promise<{ sessionId: string }> }
) {
  try {
    const { sessionId } = await params
    
    // Use the new axios-based request utility
    const apiResponse = await serverApiRequest({
      method: 'POST',
      url: `/sessions/${sessionId}/simulate`,
      data: {}, // Send empty JSON object to satisfy the backend's JSON requirement
    })

    // The data is in apiResponse.data
    return NextResponse.json(apiResponse.data, { status: 200 })

  } catch (error) {
    // Handle errors, including Axios-specific ones
    if (error instanceof AxiosError) {
      return NextResponse.json(
        { error: 'Failed to simulate game' },
        { status: error.response?.status || 500 }
      )
    }
    return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 })
  }
} 