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

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // Get the Aspire-injected service URL for client-side use
  let gameSessionUrl = process.env['services__game-session__http__0'] || 
                      process.env.GAME_SESSION_SERVICE_URL || 
                      'http://localhost:8081';
  
  // In production, use HTTPS; in development, use HTTP
  const isDevelopment = process.env.NODE_ENV === 'development';
  if (!isDevelopment && gameSessionUrl.startsWith('http://')) {
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
