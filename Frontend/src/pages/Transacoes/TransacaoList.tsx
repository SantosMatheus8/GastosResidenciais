import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { transacaoApi, pessoaApi, categoriaApi } from '../../services/api'
import { TipoTransacaoLabel } from '../../types'
import { formatCurrency } from '../../utils/format'
import ConfirmDialog from '../../components/ConfirmDialog'

export default function TransacaoList() {
  const queryClient = useQueryClient()
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [erro, setErro] = useState('')

  const { data: transacoes, isLoading } = useQuery({
    queryKey: ['transacoes'],
    queryFn: () => transacaoApi.listar(),
  })

  // Busca pessoas e categorias para exibir nomes na tabela
  const { data: pessoas } = useQuery({
    queryKey: ['pessoas'],
    queryFn: () => pessoaApi.listar(1, 200),
  })

  const { data: categorias } = useQuery({
    queryKey: ['categorias'],
    queryFn: () => categoriaApi.listar(1, 200),
  })

  const deleteMutation = useMutation({
    mutationFn: transacaoApi.deletar,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transacoes'] })
      setDeleteId(null)
      setErro('')
    },
    onError: (err: Error) => {
      setErro(err.message)
      setDeleteId(null)
    },
  })

  // Mapas para lookup rápido de nomes
  const pessoaMap = new Map(pessoas?.lines.map((p) => [p.id, p.nome]) ?? [])
  const categoriaMap = new Map(categorias?.lines.map((c) => [c.id, c.descricao]) ?? [])

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-gray-800">Transações</h2>
        <Link
          to="/transacoes/novo"
          className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 text-sm font-medium"
        >
          Nova Transação
        </Link>
      </div>

      {erro && (
        <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
          {erro}
        </div>
      )}

      {isLoading ? (
        <p className="text-gray-500">Carregando...</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Descrição</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Pessoa</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Categoria</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tipo</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Valor</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {transacoes?.lines.map((t) => (
                <tr key={t.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm text-gray-900">{t.descricao}</td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    {pessoaMap.get(t.pessoaId) ?? '—'}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    {categoriaMap.get(t.categoriaId) ?? '—'}
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <span
                      className={`inline-block px-2 py-1 rounded-full text-xs font-medium ${
                        t.tipo === 0
                          ? 'bg-red-100 text-red-700'
                          : 'bg-green-100 text-green-700'
                      }`}
                    >
                      {TipoTransacaoLabel[t.tipo] ?? t.tipo}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-right font-medium">
                    {formatCurrency(t.valor)}
                  </td>
                  <td className="px-6 py-4 text-right space-x-2">
                    <Link
                      to={`/transacoes/${t.id}/editar`}
                      className="text-indigo-600 hover:text-indigo-800 text-sm font-medium"
                    >
                      Editar
                    </Link>
                    <button
                      onClick={() => setDeleteId(t.id)}
                      className="text-red-600 hover:text-red-800 text-sm font-medium"
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
              {transacoes?.lines.length === 0 && (
                <tr>
                  <td colSpan={6} className="px-6 py-8 text-center text-gray-500">
                    Nenhuma transação cadastrada.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmDialog
        open={deleteId !== null}
        title="Excluir Transação"
        message="Tem certeza que deseja excluir esta transação?"
        onConfirm={() => deleteId && deleteMutation.mutate(deleteId)}
        onCancel={() => setDeleteId(null)}
      />
    </div>
  )
}
