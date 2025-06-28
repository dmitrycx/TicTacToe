# WebUI.Dockerfile

# 1. Build Stage
FROM node:20-alpine AS builder
WORKDIR /app
COPY src/TicTacToe.WebUI/package*.json ./
RUN npm install
COPY src/TicTacToe.WebUI/ .
RUN npm run build

# 2. Production Stage
FROM node:20-alpine AS runner
WORKDIR /app
ENV NODE_ENV=production
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

EXPOSE 3000
CMD ["node", "server.js"] 