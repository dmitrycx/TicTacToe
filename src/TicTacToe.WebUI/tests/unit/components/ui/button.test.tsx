import { render, screen, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Button } from '@/components/ui/button'

describe('Button Component', () => {
  it('renders with default variant', async () => {
    await act(async () => {
      render(<Button>Click me</Button>)
    })
    const button = screen.getByRole('button', { name: 'Click me' })
    expect(button).toBeInTheDocument()
  })

  it('renders with different variants', async () => {
    let rerender;
    await act(async () => {
      const utils = render(<Button variant="destructive">Delete</Button>)
      rerender = utils.rerender
    })
    let button = screen.getByRole('button', { name: 'Delete' })
    expect(button).toBeInTheDocument()

    await act(async () => {
      rerender!(<Button variant="outline">Outline</Button>)
    })
    button = screen.getByRole('button', { name: 'Outline' })
    expect(button).toBeInTheDocument()

    await act(async () => {
      rerender!(<Button variant="secondary">Secondary</Button>)
    })
    button = screen.getByRole('button', { name: 'Secondary' })
    expect(button).toBeInTheDocument()

    await act(async () => {
      rerender!(<Button variant="ghost">Ghost</Button>)
    })
    button = screen.getByRole('button', { name: 'Ghost' })
    expect(button).toBeInTheDocument()

    await act(async () => {
      rerender!(<Button variant="link">Link</Button>)
    })
    button = screen.getByRole('button', { name: 'Link' })
    expect(button).toBeInTheDocument()
  })

  it('renders with different sizes', async () => {
    let rerender;
    await act(async () => {
      const utils = render(<Button size="sm">Small</Button>)
      rerender = utils.rerender
    })
    let button = screen.getByRole('button', { name: 'Small' })
    expect(button).toBeInTheDocument()

    await act(async () => {
      rerender!(<Button size="lg">Large</Button>)
    })
    button = screen.getByRole('button', { name: 'Large' })
    expect(button).toBeInTheDocument()

    await act(async () => {
      rerender!(<Button size="icon">Icon</Button>)
    })
    button = screen.getByRole('button', { name: 'Icon' })
    expect(button).toBeInTheDocument()
  })

  it('handles click events', async () => {
    const handleClick = jest.fn()
    const user = userEvent.setup()
    await act(async () => {
      render(<Button onClick={handleClick}>Click me</Button>)
    })
    const button = screen.getByRole('button', { name: 'Click me' })
    await act(async () => {
      await user.click(button)
    })
    expect(handleClick).toHaveBeenCalledTimes(1)
  })

  it('can be disabled', async () => {
    await act(async () => {
      render(<Button disabled>Disabled</Button>)
    })
    const button = screen.getByRole('button', { name: 'Disabled' })
    expect(button).toBeDisabled()
  })

  it('forwards ref correctly', async () => {
    const ref = jest.fn()
    await act(async () => {
      render(<Button ref={ref}>Ref Button</Button>)
    })
    expect(ref).toHaveBeenCalled()
  })

  it('applies custom className', async () => {
    await act(async () => {
      render(<Button className="custom-class">Custom</Button>)
    })
    const button = screen.getByRole('button', { name: 'Custom' })
    expect(button).toHaveClass('custom-class')
  })

  it('renders as different elements', async () => {
    await act(async () => {
      render(<Button asChild><a href="/test">Link Button</a></Button>)
    })
    const link = screen.getByRole('link', { name: 'Link Button' })
    expect(link).toBeInTheDocument()
    expect(link).toHaveAttribute('href', '/test')
  })
}) 