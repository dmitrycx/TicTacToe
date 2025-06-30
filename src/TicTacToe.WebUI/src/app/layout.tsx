import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "TicTacToe Game",
  description: "A real-time TicTacToe game with AI strategies",
};

// Force dynamic rendering to read environment variables at runtime
export const dynamic = 'force-dynamic';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // Log all environment variables for debugging
  console.log('Available environment variables:', Object.keys(process.env));
  
  // Get the Aspire-injected service URL for client-side use
  // Try multiple possible environment variable names
  let gameSessionUrl = process.env['services__game-session__http__0'] || 
                      process.env['SERVICES__GAME_SESSION__HTTP__0'] ||
                      process.env.GAME_SESSION_SERVICE_URL || 
                      'http://localhost:8081';
  
  console.log('Using Aspire-injected service URL:', gameSessionUrl);
  
  // For container-to-container communication, use HTTP, not HTTPS
  // Only convert to HTTPS for external access in production
  const isDevelopment = process.env.NODE_ENV === 'development';
  const isContainerEnvironment = process.env.container === 'podman' || process.env.container === 'docker';
  
  // If we're in a container environment, keep HTTP for internal communication
  if (isContainerEnvironment && gameSessionUrl.startsWith('http://')) {
    // Keep as HTTP for container-to-container communication
  } else if (!isDevelopment && gameSessionUrl.startsWith('http://')) {
    // Only convert to HTTPS for external production access
    gameSessionUrl = gameSessionUrl.replace('http://', 'https://');
  }

  return (
    <html lang="en">
      <head>
        {/* Expose the service URL to the client for SignalR */}
        <script
          dangerouslySetInnerHTML={{
            __html: `window.GAME_SESSION_SERVICE_URL = "${gameSessionUrl}";`,
          }}
        />
      </head>
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        {children}
      </body>
    </html>
  );
}
