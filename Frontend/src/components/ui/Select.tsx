import { forwardRef, type SelectHTMLAttributes, type ReactNode } from 'react'

interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label: string
  error?: string
  children: ReactNode
}

// Select com label e mensagem de erro integrada.
// Usa forwardRef para compatibilidade com react-hook-form register().
const Select = forwardRef<HTMLSelectElement, SelectProps>(
  ({ label, error, children, className = '', ...props }, ref) => (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label}
      </label>
      <select
        ref={ref}
        className={`w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 ${className}`}
        {...props}
      >
        {children}
      </select>
      {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
    </div>
  )
)

Select.displayName = 'Select'
export default Select
