#!/bin/bash
set -e

echo "🧪 Testing CI pipeline locally..."

# Test frontend
echo "📝 Testing frontend..."
cd frontend

echo "  📦 Installing dependencies..."
npm ci

echo "  🔍 Linting..."
npm run lint

echo "  🧪 Running unit tests..."
npm run test:unit:ci

echo "  📊 Running coverage..."
npm run test:coverage

echo "  🔧 Type checking..."
npm run type-check

echo "  🏗️ Building..."
npm run build

cd ..

# Test backend
echo "📝 Testing backend..."

echo "  🧪 Cleaning previous builds..."
rm -rf backend/*/bin backend/*/obj 2>/dev/null || true
rm -rf backend/*/*/bin backend/*/*/obj 2>/dev/null || true

echo "  📦 Restoring dependencies..."
dotnet restore backend/BidCalculationTool.sln

echo "  🏗️ Building..."
dotnet build backend/BidCalculationTool.sln --no-restore

echo "  🧪 Running tests with coverage..."
dotnet test backend/BidCalculationTool.sln --no-build --verbosity normal --collect:"XPlat Code Coverage"

echo "✅ All tests passed! Ready to push to GitHub."