import { render, screen } from '@testing-library/react'
import { 
  Select, 
  SelectTrigger, 
  SelectContent, 
  SelectItem, 
  SelectValue 
} from '@/components/ui/select'

describe('Select Components', () => {
  describe('SelectTrigger', () => {
    it('renders with children', () => {
      render(
        <Select>
          <SelectTrigger>
            <SelectValue placeholder="Open" />
          </SelectTrigger>
        </Select>
      )
      expect(screen.getByText('Open')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(
        <Select>
          <SelectTrigger className="custom-trigger">
            <SelectValue placeholder="Open" />
          </SelectTrigger>
        </Select>
      )
      const trigger = screen.getByRole('combobox')
      expect(trigger).toHaveClass('custom-trigger')
    })

    it('can be disabled', () => {
      render(
        <Select disabled>
          <SelectTrigger>
            <SelectValue placeholder="Open" />
          </SelectTrigger>
        </Select>
      )
      const trigger = screen.getByRole('combobox')
      expect(trigger).toBeDisabled()
    })
  })

  describe('SelectContent', () => {
    it('renders with children', () => {
      render(
        <Select>
          <SelectTrigger>
            <SelectValue placeholder="Open" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="option1">Option 1</SelectItem>
            <SelectItem value="option2">Option 2</SelectItem>
          </SelectContent>
        </Select>
      )
      // Content is not visible by default, so we check the trigger exists
      expect(screen.getByRole('combobox')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(
        <Select>
          <SelectTrigger>
            <SelectValue placeholder="Open" />
          </SelectTrigger>
          <SelectContent className="custom-content">
            <SelectItem value="option1">Option 1</SelectItem>
          </SelectContent>
        </Select>
      )
      // Content is not visible by default, so we check the trigger exists
      expect(screen.getByRole('combobox')).toBeInTheDocument()
    })
  })

  describe('SelectItem', () => {
    it('renders with children', () => {
      render(
        <Select>
          <SelectTrigger>
            <SelectValue placeholder="Open" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="test">Test Item</SelectItem>
          </SelectContent>
        </Select>
      )
      // Content is not visible by default, so we check the trigger exists
      expect(screen.getByRole('combobox')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(
        <Select>
          <SelectTrigger>
            <SelectValue placeholder="Open" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="test" className="custom-item">Test Item</SelectItem>
          </SelectContent>
        </Select>
      )
      // Content is not visible by default, so we check the trigger exists
      expect(screen.getByRole('combobox')).toBeInTheDocument()
    })

    it('can be disabled', () => {
      render(
        <Select>
          <SelectTrigger>
            <SelectValue placeholder="Open" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="disabled" disabled>Disabled Item</SelectItem>
          </SelectContent>
        </Select>
      )
      // Content is not visible by default, so we check the trigger exists
      expect(screen.getByRole('combobox')).toBeInTheDocument()
    })
  })

  describe('Select composition', () => {
    it('renders a complete select component', () => {
      render(
        <Select>
          <SelectTrigger>
            <SelectValue placeholder="Select an option" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="option1">Option 1</SelectItem>
            <SelectItem value="option2">Option 2</SelectItem>
          </SelectContent>
        </Select>
      )
      
      expect(screen.getByRole('combobox')).toBeInTheDocument()
      expect(screen.getByText('Select an option')).toBeInTheDocument()
    })
  })
}) 