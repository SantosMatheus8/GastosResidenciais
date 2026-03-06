import { formatCurrency } from '../utils/format'
import type { Column } from '../components/ui'
import type { TotalPorPessoa } from '../types'

export const totaisPorPessoaColumns: Column<TotalPorPessoa>[] = [
  { header: 'Pessoa', accessor: (i) => i.nome, cellClassName: 'font-medium text-gray-900' },
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
