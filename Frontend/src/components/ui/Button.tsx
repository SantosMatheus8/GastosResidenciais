import { type ButtonHTMLAttributes, type ReactNode } from 'react'
import { Link, type To } from 'react-router-dom'

const variantStyles = {
  primary:
    'px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-50',
  secondary:
    'px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200',
  danger:
    'px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700',
  'ghost-danger': 'text-red-600 hover:text-red-800',
  'ghost-primary': 'text-indigo-600 hover:text-indigo-800',
}

type Variant = keyof typeof variantStyles
const baseStyle = 'text-sm font-medium transition-colors'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant
  children: ReactNode
}

export function Button({
  variant = 'primary',
  className = '',
  children,
  ...props
}: ButtonProps) {
  return (
    <button
      className={`${baseStyle} ${variantStyles[variant]} ${className}`}
      {...props}
    >
      {children}
    </button>
  )
}

interface LinkButtonProps {
  to: To
  variant?: Variant
  className?: string
  children: ReactNode
}

export function LinkButton({
  to,
  variant = 'primary',
  className = '',
  children,
}: LinkButtonProps) {
  return (
    <Link
      to={to}
      className={`inline-block ${baseStyle} ${variantStyles[variant]} ${className}`}
    >
      {children}
    </Link>
  )
}
