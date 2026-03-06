import { useQuery } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'
import { formatCurrency } from '../../utils/format'
import { PageHeader, DataTable } from '../../components/ui'
import { totaisPorPessoaColumns } from '../../columns'

export default function TotaisPorPessoa() {
  const { data, isLoading } = useQuery({
    queryKey: ['totais-pessoa'],
    queryFn: pessoaApi.totais,
  })

  const footer = data && (
    <tr className="bg-gray-100 font-bold">
      <td className="px-6 py-4 text-sm text-gray-900">Total Geral</td>
      <td className="px-6 py-4 text-sm text-right text-green-700">{formatCurrency(data.totalGeralReceitas)}</td>
      <td className="px-6 py-4 text-sm text-right text-red-700">{formatCurrency(data.totalGeralDespesas)}</td>
      <td className={`px-6 py-4 text-sm text-right ${data.saldoLiquido >= 0 ? 'text-green-700' : 'text-red-700'}`}>
        {formatCurrency(data.saldoLiquido)}
      </td>
    </tr>
  )

  return (
    <div>
      <PageHeader title="Totais por Pessoa" />

      <DataTable
        columns={totaisPorPessoaColumns}
        data={data?.itens ?? []}
        keyExtractor={(i) => i.pessoaId}
        isLoading={isLoading}
        footer={footer}
      />
    </div>
  )
}
