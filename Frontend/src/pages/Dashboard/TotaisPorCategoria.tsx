import { useQuery } from '@tanstack/react-query'
import { categoriaApi } from '../../services/api'
import { FinalidadeLabel } from '../../types'
import { formatCurrency } from '../../utils/format'

export default function TotaisPorCategoria() {
  const { data, isLoading } = useQuery({
    queryKey: ['totais-categoria'],
    queryFn: categoriaApi.totais,
  })

  return (
    <div>
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Totais por Categoria</h2>

      {isLoading ? (
        <p className="text-gray-500">Carregando...</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Categoria</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Finalidade</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total Receitas</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Total Despesas</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Saldo</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {data?.itens.map((item) => (
                <tr key={item.categoriaId} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm font-medium text-gray-900">{item.descricao}</td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    <span
                      className={`inline-block px-2 py-1 rounded-full text-xs font-medium ${
                        item.finalidade === 0
                          ? 'bg-red-100 text-red-700'
                          : item.finalidade === 1
                          ? 'bg-green-100 text-green-700'
                          : 'bg-blue-100 text-blue-700'
                      }`}
                    >
                      {FinalidadeLabel[item.finalidade] ?? item.finalidade}
                    </span>
                  </td>
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
                  <td className="px-6 py-4" />
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
