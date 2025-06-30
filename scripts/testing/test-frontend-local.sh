#!/bin/bash

# Frontend Local Testing Script
# This script mirrors the CI environment to catch frontend test issues locally

set -e  # Exit on any error

echo "🧪 Running Frontend Tests (CI-like environment)..."

# Navigate to WebUI directory
cd src/TicTacToe.WebUI

echo "📦 Installing dependencies..."
npm ci

echo "🔧 Installing Playwright browsers..."
npx playwright install --with-deps chromium

echo "🧪 Running unit tests..."
npm test -- --coverage --watchAll=false --passWithNoTests

echo "🌐 Running E2E tests..."
npx playwright test --project=chromium --reporter=list

echo "✅ Frontend tests completed successfully!" 