# UI Testing Guide

This document outlines the testing strategy for the TicTacToe WebUI project.

## 🧪 Testing Strategy Overview

We use a **three-tier testing approach** to ensure comprehensive coverage:

1. **Unit Tests** (Jest + React Testing Library) - Fast, isolated component testing
2. **Integration Tests** (Jest + MSW) - API integration testing with mocked services
3. **E2E Tests** (Playwright) - Full user journey testing in real browsers

## 📁 Test Structure

```
tests/
├── unit/                    # Unit tests (Jest + RTL)
│   ├── components/         # React component tests
│   ├── services/           # Service layer tests
│   └── utils/              # Utility function tests
├── integration/            # Integration tests (Jest + MSW)
│   ├── api/               # API integration tests
│   └── signalr/           # SignalR integration tests
└── e2e/                   # End-to-end tests (Playwright)
    ├── game-flow.spec.ts  # Complete game scenarios
    └── ui-components.spec.ts # UI interaction tests
```

## 🚀 Quick Start

### Install Dependencies

```bash
npm install
```

### Run Tests

```bash
# Unit tests
npm test

# Unit tests with coverage
npm run test:coverage

# Unit tests in watch mode
npm run test:watch

# E2E tests
npm run test:e2e

# E2E tests with UI
npm run test:e2e:ui

# E2E tests in headed mode
npm run test:e2e:headed
```

## 🧩 Unit Testing (Jest + React Testing Library)

### Best Practices

1. **Test Behavior, Not Implementation**
   ```tsx
   // ✅ Good - Test what user sees
   expect(screen.getByText('Create New Session')).toBeInTheDocument()
   
   // ❌ Bad - Test implementation details
   expect(component.state.isLoading).toBe(true)
   ```

2. **Use Semantic Queries**
   ```tsx
   // ✅ Good - Use accessible queries
   screen.getByRole('button', { name: 'Create Session' })
   screen.getByLabelText('Session name')
   
   // ❌ Bad - Use implementation queries
   screen.getByTestId('create-button')
   ```

3. **Test User Interactions**
   ```tsx
   const user = userEvent.setup()
   await user.click(screen.getByText('Create New Session'))
   await user.type(screen.getByLabelText('Name'), 'Test Session')
   ```

### Component Testing Example

```tsx
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import TicTacToeGame from '@/components/TicTacToeGame'

describe('TicTacToeGame', () => {
  it('creates a new session when button is clicked', async () => {
    const user = userEvent.setup()
    render(<TicTacToeGame />)
    
    await user.click(screen.getByText('Create New Session'))
    
    await waitFor(() => {
      expect(screen.getByText('Session created!')).toBeInTheDocument()
    })
  })
})
```

## 🔗 Integration Testing (MSW)

### API Mocking with MSW

```tsx
import { rest } from 'msw'
import { setupServer } from 'msw/node'

const server = setupServer(
  rest.get('/api/sessions', (req, res, ctx) => {
    return res(
      ctx.json([
        { id: '1', status: 'Completed' },
        { id: '2', status: 'InProgress' }
      ])
    )
  }),
  
  rest.post('/api/sessions', (req, res, ctx) => {
    return res(
      ctx.json({ id: '3', status: 'Created' })
    )
  })
)

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())
```

### SignalR Mocking

```tsx
// Mock SignalR connection
jest.mock('@/services/signalr', () => ({
  useSignalRConnection: jest.fn(() => ({
    connection: {
      state: 'Connected',
      on: jest.fn(),
      off: jest.fn(),
    },
    isConnected: true,
  })),
}))
```

## 🌐 E2E Testing (Playwright)

### Test Structure

```tsx
import { test, expect } from '@playwright/test'

test.describe('Game Flow', () => {
  test('complete game simulation', async ({ page }) => {
    await page.goto('/')
    
    // Create session
    await page.getByRole('button', { name: 'Create New Session' }).click()
    await expect(page.getByText(/Session \d+/)).toBeVisible()
    
    // Start simulation
    await page.getByRole('button', { name: 'Simulate' }).click()
    await expect(page.getByText('Completed')).toBeVisible()
  })
})
```

### Best Practices

1. **Use Semantic Locators**
   ```tsx
   // ✅ Good
   page.getByRole('button', { name: 'Create Session' })
   page.getByLabelText('Session name')
   
   // ❌ Bad
   page.locator('#create-btn')
   page.locator('.btn-primary')
   ```

2. **Add Data Attributes for Complex Elements**
   ```tsx
   // In component
   <div data-testid="game-board">
     {cells.map(cell => (
       <div key={cell.id} data-testid="board-cell">
         {cell.value}
       </div>
     ))}
   </div>
   
   // In test
   await expect(page.locator('[data-testid="game-board"]')).toBeVisible()
   ```

3. **Handle Async Operations**
   ```tsx
   // Wait for API calls
   await expect(page.getByText('Session created')).toBeVisible({ timeout: 10000 })
   
   // Wait for animations
   await page.waitForTimeout(1000)
   ```

## 📊 Test Coverage

### Coverage Goals

- **Statements**: 80%
- **Branches**: 75%
- **Functions**: 80%
- **Lines**: 80%

### Coverage Report

```bash
npm run test:coverage
```

This generates a coverage report in `coverage/lcov-report/index.html`

## 🔧 Test Configuration

### Jest Configuration (`jest.config.js`)

- Uses `next/jest` for Next.js compatibility
- JSDOM environment for DOM testing
- Coverage thresholds enforced
- Module path mapping for clean imports

### Playwright Configuration (`playwright.config.ts`)

- Multiple browser support (Chrome, Firefox, Safari)
- Mobile viewport testing
- Automatic dev server startup
- Screenshot and trace capture on failure

## 🚨 Common Testing Patterns

### Testing Async Operations

```tsx
// Wait for API response
await waitFor(() => {
  expect(screen.getByText('Session created')).toBeInTheDocument()
})

// Wait for loading state to disappear
await waitForElementToBeRemoved(() => 
  screen.getByText('Creating session...')
)
```

### Testing Error States

```tsx
// Mock API error
const { apiClient } = require('@/services/api-client')
apiClient.sessions.create.mockRejectedValue(new Error('Network error'))

// Test error handling
await user.click(screen.getByText('Create New Session'))
await expect(screen.getByText(/error/i)).toBeInTheDocument()
```

### Testing Real-time Updates

```tsx
// Mock SignalR events
const mockConnection = {
  on: jest.fn((event, callback) => {
    if (event === 'MoveMade') {
      callback({ position: 0, player: 'X' })
    }
  })
}

// Test real-time updates
await waitFor(() => {
  expect(screen.getByText('X')).toBeInTheDocument()
})
```

## 🎯 Testing Checklist

### Before Writing Tests

- [ ] Understand the component/service behavior
- [ ] Identify user interactions and edge cases
- [ ] Plan test scenarios and expected outcomes
- [ ] Consider error states and loading states

### When Writing Tests

- [ ] Use semantic queries (getByRole, getByLabelText)
- [ ] Test user behavior, not implementation
- [ ] Mock external dependencies
- [ ] Handle async operations properly
- [ ] Test error scenarios

### After Writing Tests

- [ ] Run tests locally
- [ ] Check coverage report
- [ ] Ensure tests are maintainable
- [ ] Update documentation if needed

## 🐛 Debugging Tests

### Jest Debugging

```bash
# Run specific test with verbose output
npm test -- --verbose --testNamePattern="creates session"

# Debug with Node.js debugger
node --inspect-brk node_modules/.bin/jest --runInBand
```

### Playwright Debugging

```bash
# Run tests in headed mode
npm run test:e2e:headed

# Run with Playwright UI
npm run test:e2e:ui

# Debug specific test
npx playwright test game-flow.spec.ts --debug
```

## 📈 CI/CD Integration

### GitHub Actions Example

```yaml
- name: Run Unit Tests
  run: npm test -- --coverage --watchAll=false

- name: Run E2E Tests
  run: npm run test:e2e

- name: Upload Coverage
  uses: codecov/codecov-action@v3
```

## 🔄 Continuous Testing

### Pre-commit Hooks

```json
{
  "husky": {
    "hooks": {
      "pre-commit": "npm test -- --watchAll=false",
      "pre-push": "npm run test:e2e"
    }
  }
}
```

This testing strategy ensures your UI is robust, maintainable, and provides a great user experience! 