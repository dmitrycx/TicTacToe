import { test, expect } from '@playwright/test'

test.describe('TicTacToe Game Basic Rendering', () => {
  test('displays main header and game board', async ({ page }) => {
    await page.goto('/')
    await expect(page.getByTestId('main-header')).toBeVisible()
    await expect(page.getByTestId('game-board')).toBeVisible()
    const cells = page.locator('[data-testid^="board-cell-"]')
    await expect(cells).toHaveCount(9)
  })
}) 