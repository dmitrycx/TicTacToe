import { NextResponse } from 'next/server'
import { serverApiRequest } from '@/lib/server-api'
import { AxiosError } from 'axios'

// Server-side environment variable (not exposed to client)
const GAME_SESSION_SERVICE_URL = process.env.GAME_SESSION_SERVICE_URL || 'http://localhost:5002'

/**
 * @swagger
 * /api/sessions:
 *   get:
 *     summary: Retrieve all game sessions
 *     description: Acts as a BFF to fetch a list of all game sessions from the backend GameSession microservice.
 *     responses:
 *       200:
 *         description: Successfully retrieved sessions list.
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 sessions:
 *                   type: array
 *                   items:
 *                     $ref: '#/components/schemas/SessionSummary'
 *       500:
 *         description: Internal server error.
 */
export async function GET() {
  try {
    // Use the new axios-based request utility
    const apiResponse = await serverApiRequest({
      method: 'GET',
      url: '/sessions', // The path is relative to the baseURL configured in axios
    })

    // The data is in apiResponse.data
    return NextResponse.json(apiResponse.data)

  } catch (error) {
    // Handle errors, including Axios-specific ones
    if (error instanceof AxiosError) {
      return NextResponse.json(
        { error: 'Failed to retrieve sessions' },
        { status: error.response?.status || 500 }
      )
    }
    return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 })
  }
}

/**
 * @swagger
 * /api/sessions:
 *   post:
 *     summary: Create a new game session
 *     description: Acts as a BFF to create a new game session in the backend GameSession microservice.
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               player1Name:
 *                 type: string
 *                 description: Name of the first player
 *               player2Name:
 *                 type: string
 *                 description: Name of the second player
 *               gameType:
 *                 type: string
 *                 enum: [HumanVsHuman, HumanVsComputer, ComputerVsComputer]
 *                 description: Type of game to create
 *     responses:
 *       201:
 *         description: Session created successfully.
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/CreateSessionResponse'
 *       400:
 *         description: Invalid request data.
 *       500:
 *         description: Internal server error.
 */
export async function POST(request: Request) {
  try {
    const body = await request.json()
    
    // Use the new axios-based request utility
    const apiResponse = await serverApiRequest({
      method: 'POST',
      url: '/sessions', // The path is relative to the baseURL configured in axios
      data: body,
    })
    
    // The created data is in apiResponse.data
    return NextResponse.json(apiResponse.data, { status: 201 })

  } catch (error) {
    // Handle errors, including Axios-specific ones
    if (error instanceof AxiosError) {
      return NextResponse.json(
        { error: 'Failed to create session' },
        { status: error.response?.status || 500 }
      )
    }
    return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 })
  }
} 