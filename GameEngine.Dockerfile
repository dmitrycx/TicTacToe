# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["src/TicTacToe.GameEngine/TicTacToe.GameEngine.csproj", "src/TicTacToe.GameEngine/"]
COPY ["aspire/TicTacToe.ServiceDefaults/TicTacToe.ServiceDefaults.csproj", "aspire/TicTacToe.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "src/TicTacToe.GameEngine/TicTacToe.GameEngine.csproj"

# Copy source code
COPY ["src/TicTacToe.GameEngine/", "src/TicTacToe.GameEngine/"]
COPY ["aspire/TicTacToe.ServiceDefaults/", "aspire/TicTacToe.ServiceDefaults/"]

# Build and publish
WORKDIR "/src/src/TicTacToe.GameEngine"
RUN dotnet publish "TicTacToe.GameEngine.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# --- BEST PRACTICE ---
# Add common, lightweight diagnostic tools needed for a container environment.
# 'curl' is for health checks. 'procps' can be useful for debugging running processes.
# Use --no-install-recommends to keep the image small.
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl procps && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser

# Copy published application
COPY --from=build --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Health check - use /alive for liveness checks
HEALTHCHECK --interval=10s --timeout=3s --retries=5 --start-period=30s \
    CMD curl -f http://localhost:8080/alive || exit 1

ENTRYPOINT ["dotnet", "TicTacToe.GameEngine.dll"] 