import { useEffect, useMemo } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { transacaoApi, pessoaApi, categoriaApi } from '../../services/api'
import { Finalidade, TipoTransacao } from '../../types'

// Validação Zod para Transação
const transacaoSchema = z.object({
  descricao: z
    .string()
    .min(1, 'Descrição é obrigatória')
    .max(400, 'Descrição deve ter no máximo 400 caracteres'),
  valor: z
    .number({ invalid_type_error: 'Valor deve ser um número' })
    .positive('Valor deve ser maior que 0'),
  tipo: z.coerce.number().int().min(0).max(1),
  categoriaId: z.string().uuid('Selecione uma categoria'),
  pessoaId: z.string().uuid('Selecione uma pessoa'),
})

type TransacaoFormData = z.infer<typeof transacaoSchema>

export default function TransacaoForm() {
  const { id } = useParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEditing = Boolean(id)

  const {
    register,
    handleSubmit,
    reset,
    control,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<TransacaoFormData>({
    resolver: zodResolver(transacaoSchema),
    defaultValues: {
      descricao: '',
      valor: 0,
      tipo: 0,
      categoriaId: '',
      pessoaId: '',
    },
  })

  // Observa o campo "tipo" para filtrar categorias compatíveis
  const tipoSelecionado = useWatch({ control, name: 'tipo' })

  const { data: transacao } = useQuery({
    queryKey: ['transacao', id],
    queryFn: () => transacaoApi.buscar(id!),
    enabled: isEditing,
  })

  const { data: pessoas } = useQuery({
    queryKey: ['pessoas'],
    queryFn: () => pessoaApi.listar(1, 200),
  })

  const { data: categorias } = useQuery({
    queryKey: ['categorias'],
    queryFn: () => categoriaApi.listar(1, 200),
  })

  // Filtra categorias pela compatibilidade com o tipo selecionado:
  // Tipo Despesa (0) → categorias com finalidade Despesa (0) ou Ambas (2)
  // Tipo Receita (1) → categorias com finalidade Receita (1) ou Ambas (2)
  const categoriasFiltradas = useMemo(() => {
    if (!categorias?.lines) return []
    const tipo = Number(tipoSelecionado)
    return categorias.lines.filter((c) => {
      if (c.finalidade === Finalidade.Ambas) return true
      if (tipo === TipoTransacao.Despesa && c.finalidade === Finalidade.Despesa) return true
      if (tipo === TipoTransacao.Receita && c.finalidade === Finalidade.Receita) return true
      return false
    })
  }, [categorias, tipoSelecionado])

  useEffect(() => {
    if (transacao) {
      reset({
        descricao: transacao.descricao,
        valor: transacao.valor,
        tipo: transacao.tipo,
        categoriaId: transacao.categoriaId,
        pessoaId: transacao.pessoaId,
      })
    }
  }, [transacao, reset])

  const mutation = useMutation({
    mutationFn: (data: TransacaoFormData) =>
      isEditing ? transacaoApi.editar(id!, data) : transacaoApi.criar(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transacoes'] })
      navigate('/transacoes')
    },
    onError: (err: Error) => {
      setError('root', { message: err.message })
    },
  })

  return (
    <div className="max-w-lg">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">
        {isEditing ? 'Editar Transação' : 'Nova Transação'}
      </h2>

      <form
        onSubmit={handleSubmit((data) => mutation.mutate(data))}
        className="bg-white rounded-lg shadow p-6 space-y-4"
      >
        {errors.root && (
          <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
            {errors.root.message}
          </div>
        )}

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Descrição</label>
          <input
            {...register('descricao')}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
            placeholder="Descrição da transação"
          />
          {errors.descricao && (
            <p className="mt-1 text-sm text-red-600">{errors.descricao.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Valor (R$)</label>
          <input
            type="number"
            step="0.01"
            {...register('valor', { valueAsNumber: true })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
            placeholder="0,00"
          />
          {errors.valor && (
            <p className="mt-1 text-sm text-red-600">{errors.valor.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Tipo</label>
          <select
            {...register('tipo', { valueAsNumber: true })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
          >
            <option value={0}>Despesa</option>
            <option value={1}>Receita</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Categoria</label>
          <select
            {...register('categoriaId')}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
          >
            <option value="">Selecione uma categoria</option>
            {categoriasFiltradas.map((c) => (
              <option key={c.id} value={c.id}>
                {c.descricao}
              </option>
            ))}
          </select>
          {errors.categoriaId && (
            <p className="mt-1 text-sm text-red-600">{errors.categoriaId.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Pessoa</label>
          <select
            {...register('pessoaId')}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
          >
            <option value="">Selecione uma pessoa</option>
            {pessoas?.lines.map((p) => (
              <option key={p.id} value={p.id}>
                {p.nome}
              </option>
            ))}
          </select>
          {errors.pessoaId && (
            <p className="mt-1 text-sm text-red-600">{errors.pessoaId.message}</p>
          )}
        </div>

        <div className="flex gap-3 pt-2">
          <button
            type="submit"
            disabled={isSubmitting}
            className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 text-sm font-medium disabled:opacity-50"
          >
            {isSubmitting ? 'Salvando...' : 'Salvar'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/transacoes')}
            className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 text-sm font-medium"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
