import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  env: {
    // Expose Aspire-injected service URLs to the client
    NEXT_PUBLIC_GAME_SESSION_SERVICE_URL: process.env.services__gamesession__http__0,
    NEXT_PUBLIC_GAME_ENGINE_SERVICE_URL: process.env.services__gameengine__http__0,
  },
};

export default nextConfig;
