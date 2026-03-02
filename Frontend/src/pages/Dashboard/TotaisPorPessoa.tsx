import { useQuery } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'
import { formatCurrency } from '../../utils/format'
import { PageHeader, DataTable, type Column } from '../../components/ui'
import type { TotalPorPessoa } from '../../types'

export default function TotaisPorPessoa() {
  const { data, isLoading } = useQuery({
    queryKey: ['totais-pessoa'],
    queryFn: pessoaApi.totais,
  })

  const columns: Column<TotalPorPessoa>[] = [
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
      cellClassName: (i) => `text-right font-semibold ${i.saldo >= 0 ? 'text-green-600' : 'text-red-600'}`,
      accessor: (i) => formatCurrency(i.saldo),
    },
  ]

  // Linha de totais gerais renderizada como rodapé da tabela
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
        columns={columns}
        data={data?.itens ?? []}
        keyExtractor={(i) => i.pessoaId}
        isLoading={isLoading}
        footer={footer}
      />
    </div>
  )
}
