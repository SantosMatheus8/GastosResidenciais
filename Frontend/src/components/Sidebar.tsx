import { NavLink, Link } from 'react-router-dom'

// === Tipos de itens de navegação ===
type NavGroup = { label: string; children: { to: string; label: string }[] }
type NavLinkItem = { to: string; label: string }
type NavItem = NavGroup | NavLinkItem

function isGroup(item: NavItem): item is NavGroup {
  return 'children' in item
}

// Itens do menu lateral
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

// Classe CSS do link ativo/inativo no menu
const linkClass = ({ isActive }: { isActive: boolean }) =>
  `block px-4 py-2 rounded-lg transition-colors ${
    isActive
      ? 'bg-indigo-600 text-white'
      : 'text-gray-300 hover:bg-gray-700 hover:text-white'
  }`

interface SidebarProps {
  open: boolean
  onClose: () => void
}

export default function Sidebar({ open, onClose }: SidebarProps) {
  return (
    <>
      {open && (
        <div
          className="fixed inset-0 bg-black/50 z-20 lg:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`fixed lg:static inset-y-0 left-0 z-30 w-64 bg-gray-800 transform transition-transform lg:translate-x-0 ${
          open ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <Link
          to="/"
          className="block p-4 border-b border-gray-700 hover:bg-gray-700/50 transition-colors"
          onClick={onClose}
        >
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
                      onClick={onClose}
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
                onClick={onClose}
              >
                {item.label}
              </NavLink>
            )
          )}
        </nav>
      </aside>
    </>
  )
}
