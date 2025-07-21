# Dockerfile for BidCalculationTool (Production)
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /app

# Copy solution file
COPY BidCalculationTool.sln ./

# Copy all project files
COPY src/BidCalculationTool.Api/BidCalculationTool.Api.csproj ./src/BidCalculationTool.Api/
COPY src/BidCalculationTool.Application/BidCalculationTool.Application.csproj ./src/BidCalculationTool.Application/
COPY src/BidCalculationTool.Domain/BidCalculationTool.Domain.csproj ./src/BidCalculationTool.Domain/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY src/ ./src/

# Build and publish
RUN dotnet publish src/BidCalculationTool.Api/BidCalculationTool.Api.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "BidCalculationTool.Api.dll"]