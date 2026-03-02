import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'
import ConfirmDialog from '../../components/ConfirmDialog'

export default function PessoaList() {
  const queryClient = useQueryClient()
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [erro, setErro] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['pessoas'],
    queryFn: () => pessoaApi.listar(),
  })

  const deleteMutation = useMutation({
    mutationFn: pessoaApi.deletar,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pessoas'] })
      setDeleteId(null)
      setErro('')
    },
    onError: (err: Error) => {
      setErro(err.message)
      setDeleteId(null)
    },
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-gray-800">Pessoas</h2>
        <Link
          to="/pessoas/novo"
          className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 text-sm font-medium"
        >
          Nova Pessoa
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
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Nome</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Idade</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {data?.lines.map((pessoa) => (
                <tr key={pessoa.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm text-gray-900">{pessoa.nome}</td>
                  <td className="px-6 py-4 text-sm text-gray-600">{pessoa.idade}</td>
                  <td className="px-6 py-4 text-right space-x-2">
                    <Link
                      to={`/pessoas/${pessoa.id}/editar`}
                      className="text-indigo-600 hover:text-indigo-800 text-sm font-medium"
                    >
                      Editar
                    </Link>
                    <button
                      onClick={() => setDeleteId(pessoa.id)}
                      className="text-red-600 hover:text-red-800 text-sm font-medium"
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
              {data?.lines.length === 0 && (
                <tr>
                  <td colSpan={3} className="px-6 py-8 text-center text-gray-500">
                    Nenhuma pessoa cadastrada.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmDialog
        open={deleteId !== null}
        title="Excluir Pessoa"
        message="Tem certeza que deseja excluir esta pessoa? Todas as transações vinculadas também serão removidas."
        onConfirm={() => deleteId && deleteMutation.mutate(deleteId)}
        onCancel={() => setDeleteId(null)}
      />
    </div>
  )
}
