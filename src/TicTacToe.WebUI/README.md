# TicTacToe WebUI

A modern Next.js frontend for the TicTacToe game with real-time AI simulation capabilities.

## 🚀 Quick Start

### Development

```bash
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

### Testing

```bash
# Unit tests
npm test

# E2E tests (requires backend services)
npx playwright test --project=chromium

# Run all tests
npm run test:all
```

📖 **For detailed testing information, see [TESTING.md](./TESTING.md)**

## 🏗️ Project Structure

```
src/
├── app/                    # Next.js App Router
├── components/            # React components
│   └── TicTacToeGame.tsx # Main game component
├── services/              # API and SignalR services
├── lib/                   # Utility functions
└── types/                 # TypeScript type definitions

tests/
├── unit/                  # Unit tests (Jest + RTL)
├── e2e/                   # E2E tests (Playwright)
└── integration/           # Integration tests
```

## 🧪 Testing Strategy

This project uses a comprehensive testing approach:

- **Unit Tests**: Jest + React Testing Library for component testing
- **E2E Tests**: Playwright for full user journey testing
- **Integration Tests**: API and SignalR integration testing

## 🔧 Technologies

- **Framework**: Next.js 14 (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **Testing**: Jest, React Testing Library, Playwright
- **Real-time**: SignalR
- **State Management**: React hooks + Context

## 📚 Learn More

- [Next.js Documentation](https://nextjs.org/docs)
- [React Testing Library](https://testing-library.com/docs/react-testing-library/intro/)
- [Playwright Testing](https://playwright.dev/)
- [Tailwind CSS](https://tailwindcss.com/docs)

## 🚀 Deployment

The easiest way to deploy your Next.js app is to use the [Vercel Platform](https://vercel.com/new?utm_medium=default-template&filter=next.js&utm_source=create-next-app&utm_campaign=create-next-app-readme).

Check out our [Next.js deployment documentation](https://nextjs.org/docs/app/building-your-application/deploying) for more details.
