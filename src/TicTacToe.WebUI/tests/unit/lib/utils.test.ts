import { cn } from '@/lib/utils'

describe('Utils', () => {
  it('combines class names correctly', () => {
    const result = cn('class1', 'class2', 'class3')
    expect(result).toBe('class1 class2 class3')
  })

  it('handles conditional classes', () => {
    const result = cn('base-class', true && 'conditional-class', false && 'ignored-class')
    expect(result).toBe('base-class conditional-class')
  })

  it('handles undefined and null values', () => {
    const result = cn('base-class', undefined, null, 'valid-class')
    expect(result).toBe('base-class valid-class')
  })

  it('handles empty strings', () => {
    const result = cn('base-class', '', 'valid-class')
    expect(result).toBe('base-class valid-class')
  })

  it('handles objects with conditional classes', () => {
    const result = cn('base-class', {
      'conditional-class': true,
      'ignored-class': false,
    })
    expect(result).toBe('base-class conditional-class')
  })

  it('handles arrays of classes', () => {
    const result = cn('base-class', ['class1', 'class2'], 'class3')
    expect(result).toBe('base-class class1 class2 class3')
  })

  it('handles mixed input types', () => {
    const result = cn(
      'base-class',
      true && 'conditional-class',
      ['array-class1', 'array-class2'],
      {
        'object-class': true,
        'ignored-object-class': false,
      },
      'final-class'
    )
    expect(result).toBe('base-class conditional-class array-class1 array-class2 object-class final-class')
  })

  it('handles no input', () => {
    const result = cn()
    expect(result).toBe('')
  })

  it('handles single class', () => {
    const result = cn('single-class')
    expect(result).toBe('single-class')
  })
}) 