import { FinalidadeBadge, type Column } from '../components/ui'
import { formatCurrency } from '../utils/format'
import type { TotalPorCategoria } from '../types'

export const totaisPorCategoriaColumns: Column<TotalPorCategoria>[] = [
  { header: 'Categoria', accessor: (i) => i.descricao, cellClassName: 'font-medium text-gray-900' },
  { header: 'Finalidade', accessor: (i) => <FinalidadeBadge value={i.finalidade} /> },
  {
    header: 'Total Receitas',
    headerClassName: 'text-right',
    cellClassName: 'text-right text-green-600',
    accessor: (i) => formatCurrency(i.totalReceitas),
  },
  {
    header: 'Total Despesas',
    headerClassName: 'text-right',
    cellClassName: 'text-right text-red-600',
    accessor: (i) => formatCurrency(i.totalDespesas),
  },
  {
    header: 'Saldo',
    headerClassName: 'text-right',
    cellClassName: (i) =>
      `text-right font-semibold ${i.saldo >= 0 ? 'text-green-600' : 'text-red-600'}`,
    accessor: (i) => formatCurrency(i.saldo),
  },
]
