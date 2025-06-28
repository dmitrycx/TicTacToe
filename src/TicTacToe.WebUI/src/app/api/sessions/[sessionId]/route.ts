import { NextResponse } from 'next/server'
import { serverApiRequest } from '@/lib/server-api'
import { AxiosError } from 'axios'

/**
 * @swagger
 * /api/sessions/{sessionId}:
 *   get:
 *     summary: Retrieve a single game session
 *     description: Acts as a BFF to fetch the details of a specific game session by its ID from the backend GameSession microservice.
 *     parameters:
 *       - in: path
 *         name: sessionId
 *         required: true
 *         schema:
 *           type: string
 *           format: uuid
 *         description: The unique identifier of the session
 *     responses:
 *       200:
 *         description: Successfully retrieved session data.
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/GetSessionResponse'
 *       404:
 *         description: Session not found.
 *       500:
 *         description: Internal server error.
 */
export async function GET(
  request: Request,
  { params }: { params: Promise<{ sessionId: string }> }
) {
  try {
    const { sessionId } = await params
    
    // Use the new axios-based request utility
    const apiResponse = await serverApiRequest({
      method: 'GET',
      url: `/sessions/${sessionId}`,
    })

    // Check if the response data is valid
    if (!apiResponse.data) {
      return NextResponse.json({ error: 'Empty response from backend' }, { status: 500 })
    }

    // The data is in apiResponse.data
    return NextResponse.json(apiResponse.data)

  } catch (error) {
    // Handle errors, including Axios-specific ones
    if (error instanceof AxiosError) {
      // Handle cases where the session is not found (404)
      if (error.response?.status === 404) {
        return NextResponse.json({ error: 'Session not found' }, { status: 404 })
      }
      return NextResponse.json(
        { error: 'Failed to get session' },
        { status: error.response?.status || 500 }
      )
    }
    return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 })
  }
}

/**
 * @swagger
 * /api/sessions/{sessionId}:
 *   delete:
 *     summary: Delete a game session
 *     description: Acts as a BFF to delete a specific game session by its ID from the backend GameSession microservice.
 *     parameters:
 *       - in: path
 *         name: sessionId
 *         required: true
 *         schema:
 *           type: string
 *           format: uuid
 *         description: The unique identifier of the session to delete
 *     responses:
 *       204:
 *         description: Session deleted successfully.
 *       404:
 *         description: Session not found.
 *       500:
 *         description: Internal server error.
 */
export async function DELETE(
  request: Request,
  { params }: { params: Promise<{ sessionId: string }> }
) {
  try {
    const { sessionId } = await params
    
    // Use the new axios-based request utility
    await serverApiRequest({
      method: 'DELETE',
      url: `/sessions/${sessionId}`,
    })

    // Return 204 No Content for successful deletion
    return new NextResponse(null, { status: 204 })

  } catch (error) {
    // Handle errors, including Axios-specific ones
    if (error instanceof AxiosError) {
      // Handle cases where the session is not found (404)
      if (error.response?.status === 404) {
        return NextResponse.json({ error: 'Session not found' }, { status: 404 })
      }
      return NextResponse.json(
        { error: 'Failed to delete session' },
        { status: error.response?.status || 500 }
      )
    }
    return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 })
  }
} 