# Testing Guide

Complete guide for running, understanding, and troubleshooting tests in the TicTacToe project.

## 🚀 **Quick Commands (For Experienced Developers)**

### **Backend Testing**
```bash
# Unit Tests (Fast - In-Memory)
dotnet test --filter "Category=Unit"

# Integration Tests (In-Memory)
dotnet test --filter "Category=Integration"

# All Backend Tests
dotnet test
```

### **Frontend Testing**
```bash
cd src/TicTacToe.WebUI

# Unit Tests
npm test

# E2E Tests
npm run test:e2e

# All Frontend Tests
npm run test:all
```

### **Container Testing**
```bash
# Full container integration tests
./scripts/testing/test-containers-local.sh

# CI-like container testing
./scripts/testing/test-containers-ci-local.sh
```

### **Performance Optimization**
```bash
# Run tests in parallel (faster)
dotnet test --maxcpucount:0

# Run specific test projects in parallel
dotnet test tests/TicTacToe.GameEngine.Tests tests/TicTacToe.GameSession.Tests --maxcpucount:0
```

---

## 🎯 **Testing Philosophy & Strategy**

Our testing strategy follows the **Testing Pyramid** approach:
- **Unit Tests**: Fast, isolated, comprehensive coverage
- **Integration Tests**: Service interactions and API validation
- **E2E Tests**: Complete user workflows and real scenarios
- **Container Tests**: Production-like environment validation

All tests use **in-memory repositories** for fast execution and reliable results.

## 🧪 **Testing Scenarios & When to Use Each**

### **1. Unit Tests**
**When to Use**: Daily development, fast feedback, isolated testing

**Characteristics:**
- ⚡ **Speed**: Sub-second execution
- 🔒 **Isolation**: No external dependencies
- 🎯 **Focus**: Individual methods and components
- 🔄 **Frequency**: Run frequently during development

**Examples:**
- Domain logic validation
- Repository interface testing
- Business rule verification
- Component rendering tests

**Tools:**
- **Backend**: xUnit, Moq, FluentAssertions
- **Frontend**: Jest, React Testing Library

### **2. Integration Tests**
**When to Use**: API validation, service interactions, data flow

**Characteristics:**
- ⚡⚡⚡ **Speed**: Fast execution with in-memory repositories
- 🔗 **Scope**: Service interactions and API endpoints
- 🧪 **Environment**: In-memory persistence
- 📊 **Coverage**: End-to-end API workflows

**Examples:**
- API endpoint testing
- Service communication validation
- Data persistence verification
- Error handling scenarios

**Tools:**
- **Backend**: FastEndpoints.Testing, TestServer
- **Frontend**: API client testing

### **3. E2E Tests**
**When to Use**: User workflow validation, complete scenarios

**Characteristics:**
- ⚡⚡ **Speed**: Browser-based testing
- 🌐 **Scope**: Complete user journeys
- 🎭 **Realism**: Actual browser interactions
- 📱 **Coverage**: Full application stack

**Examples:**
- Complete game simulation
- User interface interactions
- Real-time updates validation
- Cross-browser compatibility

**Tools:**
- **Frontend**: Playwright, Jest
- **Backend**: TestServer integration

### **4. Container Tests**
**When to Use**: Production simulation, deployment validation

**Characteristics:**
- ⚡⚡ **Speed**: Container orchestration
- 🐳 **Environment**: Production-like containers
- 🔄 **Scope**: Full application stack
- 🚀 **Purpose**: Deployment confidence

**Examples:**
- Container health checks
- Service communication in containers
- Load balancing validation
- Production configuration testing

**Tools:**
- **Infrastructure**: Docker, Docker Compose
- **Testing**: Container integration tests

## 🚀 **Testing Workflow**

### **Development Phase**
```bash
# 1. Write code with unit tests
dotnet test --filter "Category=Unit"

# 2. Run integration tests before commits
dotnet test --filter "Category=Integration"

# 3. Run E2E tests for user workflows
cd src/TicTacToe.WebUI && npm run test:e2e
```

### **Pre-Deployment Phase**
```bash
# 1. Full test suite
dotnet test && npm run test:all

# 2. Container integration tests
./scripts/testing/test-containers-local.sh

# 3. Performance validation
dotnet test --filter "Category=Performance"
```

### **CI/CD Pipeline**
```bash
# 1. Unit and integration tests
dotnet test --filter "Category!=E2E"

# 2. Frontend tests
npm test

# 3. Container tests
./scripts/testing/test-containers-ci-local.sh

# 4. E2E tests (if needed)
npm run test:e2e:ci
```

## 🧪 **Getting Started with Testing**

### **Prerequisites**
```bash
# Ensure .NET SDK is installed
dotnet --version  # Should be 9.0.301

# Ensure Node.js is installed
node --version    # Should be 18+

# Install frontend dependencies
cd src/TicTacToe.WebUI && npm install
```

### **Quick Test Run**
```bash
# Run all backend tests
dotnet test

# Run all frontend tests
cd src/TicTacToe.WebUI && npm test

# Run E2E tests
npm run test:e2e
```

## 🧪 **Backend Testing**

### **Test Structure**
```
tests/
├── TicTacToe.GameEngine.Tests/
│   ├── Features/
│   │   ├── CreateGame/
│   │   ├── GetGameState/
│   │   └── MakeMove/
│   └── TestHelpers/
└── TicTacToe.GameSession.Tests/
    ├── Features/
    │   ├── CreateSession/
    │   ├── SimulateGame/
    │   └── ...
    └── TestHelpers/
```

### **Running Backend Tests**

#### **Unit Tests (Fast)**
```bash
# All unit tests
dotnet test --filter "Category=Unit"

# Specific service unit tests
dotnet test tests/TicTacToe.GameEngine.Tests --filter "Category=Unit"
dotnet test tests/TicTacToe.GameSession.Tests --filter "Category=Unit"

# Specific test class
dotnet test --filter "FullyQualifiedName~CreateGameDomainTests"
```

#### **Integration Tests**
```bash
# All integration tests
dotnet test --filter "Category=Integration"

# Service-specific integration tests
dotnet test tests/TicTacToe.GameEngine.Tests --filter "Category=Integration"
dotnet test tests/TicTacToe.GameSession.Tests --filter "Category=Integration"
```

#### **Container Tests**
```bash
# Container integration tests
dotnet test --filter "Category=Container"

# Or use the script
./scripts/testing/test-containers-local.sh
```

### **Understanding Backend Test Results**

#### **Test Output Example**
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.4.5+1.0.0.0 (64-bit .NET 9.0.1)
[xUnit.net 00:00:00.00]   Discovering: TicTacToe.GameEngine.Tests
[xUnit.net 00:00:00.00]   Discovered:  TicTacToe.GameEngine.Tests
[xUnit.net 00:00:00.00]   Starting:    TicTacToe.GameEngine.Tests
[xUnit.net 00:00:00.00]   Finished:    TicTacToe.GameEngine.Tests

Test Run Successful.
Total tests: 15
     Passed: 15
 Total time: 0.2345 Seconds
```

#### **Test Categories**
- **Unit**: Individual method/class testing
- **Integration**: Service interaction testing
- **Container**: Full application stack testing

## 🎨 **Frontend Testing**

### **Test Structure**
```
src/TicTacToe.WebUI/
├── tests/
│   ├── unit/
│   │   ├── components/
│   │   ├── services/
│   │   └── lib/
│   └── e2e/
│       └── game-flow.spec.ts
├── jest.config.js
└── playwright.config.ts
```

### **Running Frontend Tests**

#### **Unit Tests**
```bash
cd src/TicTacToe.WebUI

# All unit tests
npm test

# Watch mode (for development)
npm test -- --watch

# Specific test file
npm test -- TicTacToeGame.test.tsx

# With coverage
npm test -- --coverage
```

#### **E2E Tests**
```bash
cd src/TicTacToe.WebUI

# All E2E tests
npm run test:e2e

# Specific test
npm run test:e2e -- game-flow.spec.ts

# Headed mode (see browser)
npm run test:e2e -- --headed

# Debug mode
npm run test:e2e -- --debug
```

### **Understanding Frontend Test Results**

#### **Jest Unit Test Output**
```
 PASS  tests/unit/components/TicTacToeGame.test.tsx
  TicTacToeGame
    ✓ should render game board (15 ms)
    ✓ should handle cell clicks (8 ms)
    ✓ should display game status (12 ms)

Test Suites: 1 passed, 1 total
Tests:       3 passed, 3 total
Snapshots:   0 total
Time:        1.234 s
```

#### **Playwright E2E Test Output**
```
Running 1 test using 1 worker
  ✓  game-flow.spec.ts:3:1 › complete game simulation (2.3s)

1 passed (2.3s)
```

## 🔧 **Test Configuration**

### **Backend Test Configuration**
```json
// appsettings.Testing.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "UseInMemory": true
}
```

### **Frontend Test Configuration**
```javascript
// jest.config.js
module.exports = {
  testEnvironment: 'jsdom',
  setupFilesAfterEnv: ['<rootDir>/jest.setup.js'],
  testMatch: ['**/tests/unit/**/*.test.{ts,tsx}'],
  collectCoverageFrom: [
    'src/**/*.{ts,tsx}',
    '!src/**/*.d.ts'
  ]
};
```

### **Container Mode (Production Simulation)**
```bash
# Build containers first
docker build -f GameEngine.Dockerfile -t tictactoe-gameengine:local-test .
docker build -f GameSession.Dockerfile -t tictactoe-gamesession:local-test .
docker build -f ApiGateway.Dockerfile -t tictactoe-apigateway:local-test .
docker build -f WebUI.Dockerfile -t tictactoe-webui:local-test .

# Start with containers
dotnet run --project aspire/TicTacToe.AppHost -- --use-containers
```

## 📊 **Test Coverage**

### **Backend Coverage**
```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# View coverage in browser
reportgenerator -reports:TestResults/coverage.cobertura.xml -targetdir:coverage
```

### **Frontend Coverage**
```bash
cd src/TicTacToe.WebUI

# Generate coverage report
npm test -- --coverage --watchAll=false

# Coverage report is available in coverage/lcov-report/index.html
```

## 📊 **Test Coverage Strategy**

### **Backend Coverage**
- **Domain Logic**: 100% unit test coverage
- **API Endpoints**: 100% integration test coverage
- **Error Scenarios**: Comprehensive error handling tests
- **Performance**: Load and stress testing

### **Frontend Coverage**
- **Components**: Unit tests for all components
- **User Flows**: E2E tests for critical paths
- **API Integration**: Mock API testing
- **Accessibility**: Accessibility testing

### **Infrastructure Coverage**
- **Container Health**: Health check validation
- **Service Communication**: Inter-service communication
- **Configuration**: Environment-specific testing
- **Deployment**: Deployment pipeline validation

## 🎯 **Testing Best Practices**

### **Test Organization**
- **Arrange-Act-Assert**: Clear test structure
- **Test Data**: Consistent test fixtures
- **Naming**: Descriptive test names
- **Isolation**: Independent test execution

### **Performance Testing**
- **Response Times**: API response time validation
- **Throughput**: Request handling capacity
- **Memory Usage**: Memory leak detection
- **Scalability**: Load testing scenarios

### **Error Testing**
- **Exception Handling**: Proper error scenarios
- **Edge Cases**: Boundary condition testing
- **Invalid Input**: Input validation testing
- **Network Issues**: Connection failure scenarios

## 🔧 **Testing Tools and Configuration**

### **Backend Testing Stack**
```csharp
// xUnit for test framework
[Fact]
[Trait("Category", "Unit")]
public async Task CreateGame_ShouldReturnValidGame()
{
    // Arrange
    var repository = new InMemoryGameRepository();
    
    // Act
    var game = Game.Create();
    var savedGame = await repository.SaveAsync(game);
    
    // Assert
    savedGame.Should().NotBeNull();
    savedGame.Id.Should().NotBeEmpty();
}
```

### **Frontend Testing Stack**
```typescript
// Jest and React Testing Library
describe('TicTacToeGame', () => {
  it('should render game board', () => {
    render(<TicTacToeGame />);
    expect(screen.getByTestId('game-board')).toBeInTheDocument();
  });
});
```

### **E2E Testing Stack**
```typescript
// Playwright for E2E testing
test('complete game simulation', async ({ page }) => {
  await page.goto('/');
  await page.click('[data-testid="create-session"]');
  await expect(page.locator('[data-testid="game-board"]')).toBeVisible();
});
```

## 🚨 **Troubleshooting**

### **Common Backend Test Issues**

#### **Tests Fail to Start**
```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```

#### **Integration Tests Fail**
```bash
# Check if services are running
curl http://localhost:8080/health
curl http://localhost:8081/health

# Check Aspire dashboard
# Open https://localhost:17122
```

#### **Container Tests Fail**
```bash
# Check Docker
docker --version

# Clean up containers
docker system prune -f

# Rebuild containers
./scripts/development/build-local-containers.sh
```

### **Common Frontend Test Issues**

#### **Jest Tests Fail**
```bash
cd src/TicTacToe.WebUI

# Clear Jest cache
npm test -- --clearCache

# Reinstall dependencies
rm -rf node_modules package-lock.json
npm install
```

#### **E2E Tests Fail**
```bash
cd src/TicTacToe.WebUI

# Install Playwright browsers
npx playwright install

# Run with debug output
npm run test:e2e -- --debug
```

#### **TypeScript Errors**
```bash
# Check TypeScript configuration
npx tsc --noEmit

# Fix linting issues
npm run lint -- --fix
```

## 📈 **Performance Optimization**

### **Backend Test Performance**
```bash
# Run tests in parallel
dotnet test --maxcpucount:0

# Run specific test projects
dotnet test tests/TicTacToe.GameEngine.Tests tests/TicTacToe.GameSession.Tests --maxcpucount:0

# Skip slow tests during development
dotnet test --filter "Category!=Slow"
```

### **Frontend Test Performance**
```bash
cd src/TicTacToe.WebUI

# Run tests in parallel
npm test -- --maxWorkers=4

# Run only changed files
npm test -- --onlyChanged
```

### **Optimization Tips**
- **Unit tests**: Run frequently during development
- **Integration tests**: Run before commits
- **Container tests**: Run before deployment
- **E2E tests**: Run in CI/CD pipeline

## 📈 **Continuous Improvement**

### **Test Metrics**
- **Coverage**: Maintain >80% code coverage
- **Execution Time**: Keep tests under 10 seconds
- **Reliability**: Zero flaky tests
- **Maintainability**: Clear, readable test code

### **Regular Review**
- **Weekly**: Review test performance and coverage
- **Monthly**: Update testing strategy based on feedback
- **Quarterly**: Evaluate testing tools and approaches

## 🔗 **Related Documentation**

- **[Development Guide](../development/README.md)** - Development workflow
- **[Setup Guide](../setup/README.md)** - Environment setup 