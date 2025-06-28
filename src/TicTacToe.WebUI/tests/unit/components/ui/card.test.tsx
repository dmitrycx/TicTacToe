import { render, screen } from '@testing-library/react'
import { Card, CardHeader, CardFooter, CardTitle, CardDescription, CardContent } from '@/components/ui/card'

describe('Card Components', () => {
  describe('Card', () => {
    it('renders with children', () => {
      render(<Card>Card content</Card>)
      expect(screen.getByText('Card content')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(<Card className="custom-card">Card content</Card>)
      const card = screen.getByText('Card content').closest('div')
      expect(card).toHaveClass('custom-card')
    })
  })

  describe('CardHeader', () => {
    it('renders with children', () => {
      render(<CardHeader>Header content</CardHeader>)
      expect(screen.getByText('Header content')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(<CardHeader className="custom-header">Header content</CardHeader>)
      const header = screen.getByText('Header content').closest('div')
      expect(header).toHaveClass('custom-header')
    })
  })

  describe('CardTitle', () => {
    it('renders with children', () => {
      render(<CardTitle>Title content</CardTitle>)
      expect(screen.getByText('Title content')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(<CardTitle className="custom-title">Title content</CardTitle>)
      const title = screen.getByText('Title content').closest('h3')
      expect(title).toHaveClass('custom-title')
    })
  })

  describe('CardDescription', () => {
    it('renders with children', () => {
      render(<CardDescription>Description content</CardDescription>)
      expect(screen.getByText('Description content')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(<CardDescription className="custom-description">Description content</CardDescription>)
      const description = screen.getByText('Description content').closest('p')
      expect(description).toHaveClass('custom-description')
    })
  })

  describe('CardContent', () => {
    it('renders with children', () => {
      render(<CardContent>Content</CardContent>)
      expect(screen.getByText('Content')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(<CardContent className="custom-content">Content</CardContent>)
      const content = screen.getByText('Content').closest('div')
      expect(content).toHaveClass('custom-content')
    })
  })

  describe('CardFooter', () => {
    it('renders with children', () => {
      render(<CardFooter>Footer content</CardFooter>)
      expect(screen.getByText('Footer content')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(<CardFooter className="custom-footer">Footer content</CardFooter>)
      const footer = screen.getByText('Footer content').closest('div')
      expect(footer).toHaveClass('custom-footer')
    })
  })

  describe('Card composition', () => {
    it('renders complete card structure', () => {
      render(
        <Card>
          <CardHeader>
            <CardTitle>Test Title</CardTitle>
          </CardHeader>
          <CardContent>Test content</CardContent>
        </Card>
      )

      expect(screen.getByText('Test Title')).toBeInTheDocument()
      expect(screen.getByText('Test content')).toBeInTheDocument()
    })

    it('forwards ref correctly', () => {
      const ref = jest.fn()
      render(<Card ref={ref}>Card content</Card>)
      expect(ref).toHaveBeenCalled()
    })
  })
}) 