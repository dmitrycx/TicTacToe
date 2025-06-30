import '@testing-library/jest-dom'

declare global {
  namespace jest {
    interface Matchers<R> {
      toBeInTheDocument(): R
      toHaveClass(className: string): R
      toHaveAttribute(attr: string, value?: string): R
      toHaveTextContent(text: string | RegExp): R
      toBeVisible(): R
      toBeDisabled(): R
      toBeEnabled(): R
      toBeChecked(): R
      toBePartiallyChecked(): R
      toHaveValue(value: string | string[] | number): R
      toHaveDisplayValue(value: string | RegExp | (string | RegExp)[]): R
      toBeRequired(): R
      toBeInvalid(): R
      toBeValid(): R
      toHaveAccessibleDescription(expectedAccessibleDescription?: string | RegExp): R
      toHaveAccessibleName(expectedAccessibleName?: string | RegExp): R
      toHaveErrorMessage(expectedErrorMessage?: string | RegExp): R
    }
  }
}

export {} 