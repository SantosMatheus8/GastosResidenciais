import { type ReactNode, useState } from 'react'
import { NavLink, Link } from 'react-router-dom'

type NavGroup = { label: string; children: { to: string; label: string }[] }
type NavLinkItem = { to: string; label: string }
type NavItem = NavGroup | NavLinkItem

function isGroup(item: NavItem): item is NavGroup {
  return 'children' in item
}

const navItems: NavItem[] = [
  { to: '/', label: 'Início' },
  {
    label: 'Dashboard',
    children: [
      { to: '/dashboard/pessoas', label: 'Totais por Pessoa' },
      { to: '/dashboard/categorias', label: 'Totais por Categoria' },
    ],
  },
  { to: '/pessoas', label: 'Pessoas' },
  { to: '/categorias', label: 'Categorias' },
  { to: '/transacoes', label: 'Transações' },
]

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `block px-4 py-2 rounded-lg transition-colors ${
    isActive
      ? 'bg-indigo-600 text-white'
      : 'text-gray-300 hover:bg-gray-700 hover:text-white'
  }`

export default function Layout({ children }: { children: ReactNode }) {
  const [sidebarOpen, setSidebarOpen] = useState(false)

  return (
    <div className="min-h-screen flex bg-gray-100">
      {/* Overlay mobile */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-20 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed lg:static inset-y-0 left-0 z-30 w-64 bg-gray-800 transform transition-transform lg:translate-x-0 ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <Link to="/" className="block p-4 border-b border-gray-700 hover:bg-gray-700/50 transition-colors">
          <h1 className="text-xl font-bold text-white">ResiGa</h1>
          <p className="text-xs text-gray-400">Gastos Residenciais</p>
        </Link>

        <nav className="p-4 space-y-1">
          {navItems.map((item) =>
            isGroup(item) ? (
              <div key={item.label}>
                <span className="block px-4 py-2 text-xs font-semibold text-gray-400 uppercase tracking-wider">
                  {item.label}
                </span>
                <div className="ml-2 space-y-1">
                  {item.children.map((child) => (
                    <NavLink
                      key={child.to}
                      to={child.to}
                      className={linkClass}
                      onClick={() => setSidebarOpen(false)}
                    >
                      {child.label}
                    </NavLink>
                  ))}
                </div>
              </div>
            ) : (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.to === '/'}
                className={linkClass}
                onClick={() => setSidebarOpen(false)}
              >
                {item.label}
              </NavLink>
            )
          )}
        </nav>
      </aside>

      {/* Conteúdo principal */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Header mobile */}
        <header className="lg:hidden bg-white shadow px-4 py-3 flex items-center">
          <button
            onClick={() => setSidebarOpen(true)}
            className="text-gray-600 hover:text-gray-900"
          >
            <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
          <Link to="/" className="ml-3 font-semibold text-gray-800 hover:text-indigo-600 transition-colors">
            ResiGa
          </Link>
        </header>

        <main className="flex-1 p-6 overflow-auto">{children}</main>
      </div>
    </div>
  )
}
