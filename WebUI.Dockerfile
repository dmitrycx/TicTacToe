# WebUI.Dockerfile - Place in the root of your solution

# 1. Build Stage
FROM node:20-alpine AS builder

# Set the working directory inside the container
WORKDIR /app

# The context is the solution root, so specify the path to the WebUI files
COPY src/TicTacToe.WebUI/package*.json ./
RUN npm install

# Copy the rest of the WebUI source code
COPY src/TicTacToe.WebUI/ ./
RUN npm run build

# 2. Production Stage
FROM node:20-alpine AS runner
WORKDIR /app
ENV NODE_ENV=production

# The context for COPY --from is the builder stage, not the original host machine.
# So these paths are relative to /app in the builder.
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

EXPOSE 3000

# The start command for a standalone Next.js app is typically running server.js
# from the .next/standalone directory.
CMD ["node", "server.js"] 