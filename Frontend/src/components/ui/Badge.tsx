import { type ReactNode } from 'react'
import { FinalidadeLabel, TipoTransacaoLabel } from '../../types'

// Cores associadas a cada classificação
const colorStyles = {
  despesa: 'bg-red-100 text-red-700',
  receita: 'bg-green-100 text-green-700',
  ambas: 'bg-blue-100 text-blue-700',
}

type BadgeColor = keyof typeof colorStyles

interface BadgeProps {
  color: BadgeColor
  children: ReactNode
  className?: string
}

// Badge genérico com cor semântica
export function Badge({ color, children, className = '' }: BadgeProps) {
  return (
    <span
      className={`inline-block px-2 py-1 rounded-full text-xs font-medium ${colorStyles[color]} ${className}`}
    >
      {children}
    </span>
  )
}

// Badge para exibir Finalidade da categoria (Despesa / Receita / Ambas)
export function FinalidadeBadge({ value }: { value: number }) {
  const colorMap: Record<number, BadgeColor> = {
    0: 'despesa',
    1: 'receita',
    2: 'ambas',
  }
  return (
    <Badge color={colorMap[value] ?? 'ambas'}>
      {FinalidadeLabel[value] ?? value}
    </Badge>
  )
}

// Badge para exibir Tipo de transação (Despesa / Receita)
export function TipoBadge({ value }: { value: number }) {
  const colorMap: Record<number, BadgeColor> = {
    0: 'despesa',
    1: 'receita',
  }
  return (
    <Badge color={colorMap[value] ?? 'despesa'}>
      {TipoTransacaoLabel[value] ?? value}
    </Badge>
  )
}
