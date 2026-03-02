import { useQuery } from '@tanstack/react-query'
import { categoriaApi } from '../../services/api'
import { formatCurrency } from '../../utils/format'
import { PageHeader, DataTable } from '../../components/ui'
import { totaisPorCategoriaColumns } from '../../columns'

export default function TotaisPorCategoria() {
  const { data, isLoading } = useQuery({
    queryKey: ['totais-categoria'],
    queryFn: categoriaApi.totais,
  })

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
        columns={totaisPorCategoriaColumns}
        data={data?.itens ?? []}
        keyExtractor={(i) => i.categoriaId}
        isLoading={isLoading}
        footer={footer}
      />
    </div>
  )
}
