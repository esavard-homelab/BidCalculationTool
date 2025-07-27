# Multi-stage Dockerfile for Production
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build

WORKDIR /app

# Copy only source project files (no tests)
COPY src/BidCalculationTool.Api/BidCalculationTool.Api.csproj ./src/BidCalculationTool.Api/
COPY src/BidCalculationTool.Application/BidCalculationTool.Application.csproj ./src/BidCalculationTool.Application/
COPY src/BidCalculationTool.Domain/BidCalculationTool.Domain.csproj ./src/BidCalculationTool.Domain/

# Restore dependencies for source projects only
RUN dotnet restore src/BidCalculationTool.Api/BidCalculationTool.Api.csproj

# Copy source code
COPY src/ ./src/

# Build and publish the application
RUN dotnet publish src/BidCalculationTool.Api/BidCalculationTool.Api.csproj -c Release -o /app/publish

# Runtime stage - smaller image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "BidCalculationTool.Api.dll"]
