import { render, screen, act } from '@testing-library/react'
import { Label } from '@/components/ui/label'

describe('Label Component', () => {
  it('renders with children', async () => {
    await act(async () => {
      render(<Label>Test Label</Label>)
    })
    expect(screen.getByText('Test Label')).toBeInTheDocument()
  })

  it('applies custom className', async () => {
    await act(async () => {
      render(<Label className="custom-label">Custom Label</Label>)
    })
    const label = screen.getByText('Custom Label')
    expect(label).toHaveClass('custom-label')
  })

  it('forwards ref correctly', async () => {
    const ref = jest.fn()
    await act(async () => {
      render(<Label ref={ref}>Ref Label</Label>)
    })
    expect(ref).toHaveBeenCalled()
  })

  it('renders as label element', async () => {
    await act(async () => {
      render(<Label>Form Label</Label>)
    })
    const label = screen.getByText('Form Label')
    expect(label.tagName).toBe('LABEL')
  })

  it('can be associated with form controls', async () => {
    await act(async () => {
      render(
        <div>
          <Label htmlFor="input-field">Input Label</Label>
          <input id="input-field" />
        </div>
      )
    })
    const label = screen.getByText('Input Label')
    expect(label).toHaveAttribute('for', 'input-field')
  })
}) 