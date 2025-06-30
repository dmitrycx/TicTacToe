FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/TicTacToe.ApiGateway/TicTacToe.ApiGateway.csproj", "src/TicTacToe.ApiGateway/"]
RUN dotnet restore "src/TicTacToe.ApiGateway/TicTacToe.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/TicTacToe.ApiGateway"
RUN dotnet build "TicTacToe.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicTacToe.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicTacToe.ApiGateway.dll"] 