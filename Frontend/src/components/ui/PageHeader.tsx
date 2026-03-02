import { type ReactNode } from 'react'

interface PageHeaderProps {
  title: string
  children?: ReactNode 
}

// Cabeçalho de página com título e área de ações à direita
export default function PageHeader({ title, children }: PageHeaderProps) {
  return (
    <div className="flex items-center justify-between mb-6">
      <h2 className="text-2xl font-bold text-gray-800">{title}</h2>
      {children}
    </div>
  )
}
