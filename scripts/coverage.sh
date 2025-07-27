#!/bin/bash

# Script to generate a complete coverage report with coverlet + ReportGenerator
# Usage: ./scripts/coverage.sh

set -e

echo "🧪 Generate Coverage Report with coverlet and ReportGenerator..."

# Variables
BACKEND_DIR="backend"
TEST_PROJECT="test/BidCalculationTool.Test/BidCalculationTool.Test.csproj"
COVERAGE_DIR="coverage-reports"
SOLUTION_FILE="BidCalculationTool.sln"

# Create output dir
mkdir -p $COVERAGE_DIR

# Build the backend
cd $BACKEND_DIR

echo "📋 Clean up previous builds..."
dotnet clean $SOLUTION_FILE

echo "🔨 Build solution..."
dotnet build $SOLUTION_FILE --configuration Debug

echo "🧪 Run test with coverlet coverage..."

# Execute tests with coverage collection
dotnet test $TEST_PROJECT \
    --configuration Debug \
    --no-build \
    --verbosity normal \
    --collect:"XPlat Code Coverage" \
    --results-directory "../$COVERAGE_DIR" \
    --logger "console;verbosity=detailed"

echo "📊 Install ReportGenerator to generate HTML report..."
dotnet tool install --global dotnet-reportgenerator-globaltool 2>/dev/null || echo "ReportGenerator already installed"

echo "📈 Create HTML report..."
reportgenerator \
    -reports:"../$COVERAGE_DIR/**/coverage.cobertura.xml" \
    -targetdir:"../$COVERAGE_DIR/html-report" \
    -reporttypes:"Html;HtmlSummary;Badges;JsonSummary" \
    -sourcedirs:"src" \
    -title:"BidCalculationTool - Coverage Report"

echo ""
echo "✅ Coverage report generated with success !"
echo "📁 Directory: $COVERAGE_DIR/"
echo "🌐 HTML Report: $COVERAGE_DIR/html-report/index.html"
echo "📋 JSON Summary: $COVERAGE_DIR/html-report/Summary.json"

# Show quick summary
if [ -f "../$COVERAGE_DIR/html-report/Summary.json" ]; then
    echo ""
    echo "📊 Coverage summary:"
    python3 -c "
import json
import sys
try:
    with open('../$COVERAGE_DIR/html-report/Summary.json', 'r') as f:
        data = json.load(f)
    summary = data['summary']
    print(f'  📦 Global Coverage: {summary[\"linecoverage\"]:.1f}%')
    print(f'  🔀 Branches Coverage: {summary[\"branchcoverage\"]:.1f}%')
    print(f'  📝 Line Coverage: {summary[\"coveredlines\"]} / {summary[\"coverablelines\"]}')
except:
    print('  ℹ️  JSON Summary unavailable')
" 2>/dev/null || echo "  ℹ️  JSON Summary unavailable"
fi

# Open automatically the report
if command -v xdg-open &> /dev/null; then
    echo ""
    echo "🚀 Ouverture automatique du rapport..."
    xdg-open "../$COVERAGE_DIR/html-report/index.html"
fi
