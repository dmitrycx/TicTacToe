import https from 'https';
import axios, { AxiosRequestConfig } from 'axios';

const GAME_SESSION_SERVICE_URL = process.env.GAME_SESSION_SERVICE_URL;

if (!GAME_SESSION_SERVICE_URL) {
  throw new Error("Missing required environment variable: GAME_SESSION_SERVICE_URL");
}

// Create a single, reusable httpsAgent
const httpsAgent = new https.Agent({
  rejectUnauthorized: false, // For development with self-signed certs
});

// Create a single, pre-configured axios instance
const serverApiClient = axios.create({
  baseURL: GAME_SESSION_SERVICE_URL,
  httpsAgent: httpsAgent, // Use the custom agent for all requests made with this instance
  headers: {
    'Content-Type': 'application/json',
  }
});

// A wrapper function that uses the pre-configured axios instance
// This simplifies our API routes even further.
export async function serverApiRequest(config: AxiosRequestConfig) {
  try {
    const response = await serverApiClient.request(config);
    return response; // Axios response object contains data, status, headers
  } catch (error) {
    if (axios.isAxiosError(error)) {
      // Log the detailed Axios error
      console.error('BFF Axios Error:', error.response?.status, error.response?.data);
    } else {
      console.error('BFF Generic Error:', error);
    }
    // Re-throw the error to be handled by the route's try/catch block
    throw error;
  }
} 