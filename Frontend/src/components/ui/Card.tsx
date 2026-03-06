import { type ReactNode } from 'react'
import { Link, type To } from 'react-router-dom'

interface CardProps {
  to: To
  icon: ReactNode
  title: string
  description: string
}

export default function Card({ to, icon, title, description }: CardProps) {
  return (
    <Link
      to={to}
      className="flex flex-col items-center gap-2 p-6 bg-white rounded-xl shadow hover:shadow-md transition-shadow"
    >
      {icon}
      <span className="font-semibold text-gray-800">{title}</span>
      <span className="text-xs text-gray-400">{description}</span>
    </Link>
  )
}
