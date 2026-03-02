import { useQuery } from '@tanstack/react-query'
import { categoriaApi } from '../../services/api'
import { formatCurrency } from '../../utils/format'
import { PageHeader, DataTable, FinalidadeBadge, type Column } from '../../components/ui'
import type { TotalPorCategoria } from '../../types'

export default function TotaisPorCategoria() {
  const { data, isLoading } = useQuery({
    queryKey: ['totais-categoria'],
    queryFn: categoriaApi.totais,
  })

  const columns: Column<TotalPorCategoria>[] = [
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
      cellClassName: (i) => `text-right font-semibold ${i.saldo >= 0 ? 'text-green-600' : 'text-red-600'}`,
      accessor: (i) => formatCurrency(i.saldo),
    },
  ]

  const footer = data && (
    <tr className="bg-gray-100 font-bold">
      <td className="px-6 py-4 text-sm text-gray-900">Total Geral</td>
      <td className="px-6 py-4" />
      <td className="px-6 py-4 text-sm text-right text-green-700">{formatCurrency(data.totalGeralReceitas)}</td>
      <td className="px-6 py-4 text-sm text-right text-red-700">{formatCurrency(data.totalGeralDespesas)}</td>
      <td className={`px-6 py-4 text-sm text-right ${data.saldoLiquido >= 0 ? 'text-green-700' : 'text-red-700'}`}>
        {formatCurrency(data.saldoLiquido)}
      </td>
    </tr>
  )

  return (
    <div>
      <PageHeader title="Totais por Categoria" />

      <DataTable
        columns={columns}
        data={data?.itens ?? []}
        keyExtractor={(i) => i.categoriaId}
        isLoading={isLoading}
        footer={footer}
      />
    </div>
  )
}
