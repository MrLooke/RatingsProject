FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
WORKDIR /app

FROM base AS packages
COPY MediaBoard.Server.csproj .
RUN dotnet restore "MediaBoard.Server.csproj"

FROM packages AS build
COPY . .
RUN dotnet publish "MediaBoard.Server.csproj" -o ./publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "MediaBoard.Server.dll"]