# GameSession.Dockerfile - Place in the root of your solution

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["src/TicTacToe.GameEngine/TicTacToe.GameEngine.csproj", "src/TicTacToe.GameEngine/"]
COPY ["src/TicTacToe.GameSession/TicTacToe.GameSession.csproj", "src/TicTacToe.GameSession/"]

# Restore dependencies for both projects
RUN dotnet restore "src/TicTacToe.GameEngine/TicTacToe.GameEngine.csproj"
RUN dotnet restore "src/TicTacToe.GameSession/TicTacToe.GameSession.csproj"

# Copy source code for both projects
COPY ["src/TicTacToe.GameEngine/", "src/TicTacToe.GameEngine/"]
COPY ["src/TicTacToe.GameSession/", "src/TicTacToe.GameSession/"]

# Build and publish the main project (GameSession)
WORKDIR "/src/src/TicTacToe.GameSession"
RUN dotnet publish "TicTacToe.GameSession.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser

# Copy published application
COPY --from=build --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8081/health || exit 1

ENTRYPOINT ["dotnet", "TicTacToe.GameSession.dll"] 