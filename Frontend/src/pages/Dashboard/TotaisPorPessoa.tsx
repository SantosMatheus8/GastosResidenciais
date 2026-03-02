import { useQuery } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'
import { formatCurrency } from '../../utils/format'

export default function TotaisPorPessoa() {
  const { data, isLoading } = useQuery({
    queryKey: ['totais-pessoa'],
    queryFn: pessoaApi.totais,
  })

  return (
    <div>
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Totais por Pessoa</h2>

      {isLoading ? (
        <p className="text-gray-500">Carregando...</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Pessoa</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total Receitas</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total Despesas</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Saldo</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {data?.itens.map((item) => (
                <tr key={item.pessoaId} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm font-medium text-gray-900">{item.nome}</td>
                  <td className="px-6 py-4 text-sm text-right text-green-600">
                    {formatCurrency(item.totalReceitas)}
                  </td>
                  <td className="px-6 py-4 text-sm text-right text-red-600">
                    {formatCurrency(item.totalDespesas)}
                  </td>
                  <td
                    className={`px-6 py-4 text-sm text-right font-semibold ${
                      item.saldo >= 0 ? 'text-green-600' : 'text-red-600'
                    }`}
                  >
                    {formatCurrency(item.saldo)}
                  </td>
                </tr>
              ))}

              {/* Linha de totais gerais */}
              {data && (
                <tr className="bg-gray-100 font-bold">
                  <td className="px-6 py-4 text-sm text-gray-900">Total Geral</td>
                  <td className="px-6 py-4 text-sm text-right text-green-700">
                    {formatCurrency(data.totalGeralReceitas)}
                  </td>
                  <td className="px-6 py-4 text-sm text-right text-red-700">
                    {formatCurrency(data.totalGeralDespesas)}
                  </td>
                  <td
                    className={`px-6 py-4 text-sm text-right ${
                      data.saldoLiquido >= 0 ? 'text-green-700' : 'text-red-700'
                    }`}
                  >
                    {formatCurrency(data.saldoLiquido)}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
