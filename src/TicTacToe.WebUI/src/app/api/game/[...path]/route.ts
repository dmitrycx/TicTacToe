import { NextRequest } from 'next/server';

/**
 * Smart function to determine the correct GameSession URL for different environments
 */
function getGameSessionUrl(): string {
  // Strategy 1: Try Aspire service discovery (container mode)
  const aspireUrl = process.env.services__gamesession__http__0;
  if (aspireUrl) {
    console.log(`[Proxy] Using Aspire service discovery: ${aspireUrl}`);
    return aspireUrl;
  }

  // Strategy 2: Try alternative Aspire variable names
  const alternativeVars = Object.keys(process.env).filter(key => 
    key.toLowerCase().includes('gamesession') && key.toLowerCase().includes('http')
  );
  
  if (alternativeVars.length > 0) {
    const url = process.env[alternativeVars[0]];
    if (url) {
      console.log(`[Proxy] Using alternative Aspire variable ${alternativeVars[0]}: ${url}`);
      return url;
    }
  }

  // Strategy 3: Check if we're running in a container environment
  const isContainer = process.env.NODE_ENV === 'production' && 
                     (process.env.HOSTNAME || process.env.container);
  
  if (isContainer) {
    // In container mode, use the internal service name
    const containerUrl = 'http://game-session:8081';
    console.log(`[Proxy] Using container internal URL: ${containerUrl}`);
    return containerUrl;
  }

  // Strategy 4: Local development fallback - use the port from Aspire environment
  const localUrl = 'http://localhost:5001';
  console.log(`[Proxy] Using local development fallback: ${localUrl}`);
  return localUrl;
}

async function handler(req: NextRequest) {
  const targetBaseUrl = getGameSessionUrl();
  const path = req.nextUrl.pathname.replace('/api/game', '');
  const search = req.nextUrl.search;
  const targetUrl = `${targetBaseUrl}${path}${search}`;

  console.log(`[Proxy] Forwarding request to: ${targetUrl}`);

  try {
    const response = await fetch(targetUrl, {
      method: req.method,
      headers: req.headers,
      body: req.body,
      duplex: 'half'
    } as RequestInit);
    
    return response;
  } catch (error) {
    console.error(`[Proxy] Error forwarding to ${targetUrl}:`, error);
    const errorMessage = error instanceof Error ? error.message : 'Unknown error';
    return new Response(`Proxy error: ${errorMessage}`, { status: 502 });
  }
}

export { handler as GET, handler as POST, handler as PUT, handler as DELETE, handler as PATCH }; 