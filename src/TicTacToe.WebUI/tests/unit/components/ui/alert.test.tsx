import { render, screen, act } from '@testing-library/react'
import { Alert, AlertDescription } from '@/components/ui/alert'

describe('Alert Components', () => {
  describe('Alert', () => {
    it('renders with children', async () => {
      await act(async () => {
        render(<Alert>Alert content</Alert>)
      })
      expect(screen.getByText('Alert content')).toBeInTheDocument()
    })

    it('renders with different variants', async () => {
      let rerender;
      await act(async () => {
        const utils = render(<Alert variant="destructive">Destructive Alert</Alert>)
        rerender = utils.rerender
      })
      let alert = screen.getByText('Destructive Alert')
      expect(alert).toBeInTheDocument()

      await act(async () => {
        rerender!(<Alert variant="default">Default Alert</Alert>)
      })
      alert = screen.getByText('Default Alert')
      expect(alert).toBeInTheDocument()
    })

    it('applies custom className', async () => {
      await act(async () => {
        render(<Alert className="custom-alert">Custom Alert</Alert>)
      })
      const alert = screen.getByText('Custom Alert')
      expect(alert).toHaveClass('custom-alert')
    })

    it('forwards ref correctly', async () => {
      const ref = jest.fn()
      await act(async () => {
        render(<Alert ref={ref}>Ref Alert</Alert>)
      })
      expect(ref).toHaveBeenCalled()
    })
  })

  describe('AlertDescription', () => {
    it('renders with children', async () => {
      await act(async () => {
        render(<AlertDescription>Description text</AlertDescription>)
      })
      expect(screen.getByText('Description text')).toBeInTheDocument()
    })

    it('applies custom className', async () => {
      await act(async () => {
        render(<AlertDescription className="custom-description">Description text</AlertDescription>)
      })
      const description = screen.getByText('Description text')
      expect(description).toHaveClass('custom-description')
    })

    it('forwards ref correctly', async () => {
      const ref = jest.fn()
      await act(async () => {
        render(<AlertDescription ref={ref}>Ref Description</AlertDescription>)
      })
      expect(ref).toHaveBeenCalled()
    })
  })

  describe('Alert composition', () => {
    it('renders complete alert structure', async () => {
      await act(async () => {
        render(
          <Alert>
            <AlertDescription>Test description</AlertDescription>
          </Alert>
        )
      })
      expect(screen.getByText('Test description')).toBeInTheDocument()
    })
  })
}) 