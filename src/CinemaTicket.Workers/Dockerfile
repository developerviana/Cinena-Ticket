# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/CinemaTicket.Workers/CinemaTicket.Workers.csproj", "CinemaTicket.Workers/"]
COPY ["src/CinemaTicket.Application/CinemaTicket.Application.csproj", "CinemaTicket.Application/"]
COPY ["src/CinemaTicket.Domain/CinemaTicket.Domain.csproj", "CinemaTicket.Domain/"]
COPY ["src/CinemaTicket.Infrastructure/CinemaTicket.Infrastructure.csproj", "CinemaTicket.Infrastructure/"]

RUN dotnet restore "CinemaTicket.Workers/CinemaTicket.Workers.csproj"

# Copy everything else and build
COPY src/ .
WORKDIR "/src/CinemaTicket.Workers"
RUN dotnet build "CinemaTicket.Workers.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "CinemaTicket.Workers.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD dotnet CinemaTicket.Workers.dll --health || exit 1

ENTRYPOINT ["dotnet", "CinemaTicket.Workers.dll"]
