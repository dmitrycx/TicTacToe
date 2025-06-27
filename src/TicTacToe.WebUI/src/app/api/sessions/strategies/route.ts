import { NextResponse } from 'next/server';
import { serverApiRequest } from '@/lib/server-api';
import { AxiosError } from 'axios';

export async function GET() {
  try {
    // Use the serverApiRequest utility which handles SSL certificates properly
    const apiResponse = await serverApiRequest({
      method: 'GET',
      url: '/sessions/strategies',
    });

    return NextResponse.json(apiResponse.data);
  } catch (error) {
    console.error('Error fetching strategies:', error);
    
    // Handle Axios-specific errors
    if (error instanceof AxiosError) {
      return NextResponse.json(
        { error: 'Failed to fetch strategies' },
        { status: error.response?.status || 500 }
      );
    }
    
    return NextResponse.json(
      { error: 'Failed to fetch strategies' },
      { status: 500 }
    );
  }
} 