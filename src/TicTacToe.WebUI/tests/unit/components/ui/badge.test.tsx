import { render, screen, act } from '@testing-library/react'
import { Badge } from '@/components/ui/badge'

describe('Badge Component', () => {
  it('renders with default variant', async () => {
    await act(async () => {
      render(<Badge>Default Badge</Badge>)
    })
    const badge = screen.getByText('Default Badge')
    expect(badge).toBeInTheDocument()
  })

  it('renders with different variants', async () => {
    let rerender;
    await act(async () => {
      const utils = render(<Badge variant="secondary">Secondary</Badge>)
      rerender = utils.rerender
    })
    let badge = screen.getByText('Secondary')
    expect(badge).toBeInTheDocument()

    await act(async () => {
      rerender!(<Badge variant="destructive">Destructive</Badge>)
    })
    badge = screen.getByText('Destructive')
    expect(badge).toBeInTheDocument()

    await act(async () => {
      rerender!(<Badge variant="outline">Outline</Badge>)
    })
    badge = screen.getByText('Outline')
    expect(badge).toBeInTheDocument()
  })

  it('applies custom className', async () => {
    await act(async () => {
      render(<Badge className="custom-badge">Custom Badge</Badge>)
    })
    const badge = screen.getByText('Custom Badge')
    expect(badge).toHaveClass('custom-badge')
  })

  it('renders with different content types', async () => {
    await act(async () => {
      render(<Badge>Text Content</Badge>)
    })
    expect(screen.getByText('Text Content')).toBeInTheDocument()

    await act(async () => {
      render(<Badge>123</Badge>)
    })
    expect(screen.getByText('123')).toBeInTheDocument()

    await act(async () => {
      render(<Badge>Special @#$%</Badge>)
    })
    expect(screen.getByText('Special @#$%')).toBeInTheDocument()
  })
}) 