import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriaApi } from '../../services/api'

// Validação Zod para Categoria
const categoriaSchema = z.object({
  descricao: z
    .string()
    .min(1, 'Descrição é obrigatória')
    .max(400, 'Descrição deve ter no máximo 400 caracteres'),
  finalidade: z.coerce
    .number()
    .int()
    .min(0, 'Finalidade inválida')
    .max(2, 'Finalidade inválida'),
})

type CategoriaFormData = z.infer<typeof categoriaSchema>

export default function CategoriaForm() {
  const { id } = useParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEditing = Boolean(id)

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<CategoriaFormData>({
    resolver: zodResolver(categoriaSchema),
    defaultValues: { descricao: '', finalidade: 0 },
  })

  const { data: categoria } = useQuery({
    queryKey: ['categoria', id],
    queryFn: () => categoriaApi.buscar(id!),
    enabled: isEditing,
  })

  useEffect(() => {
    if (categoria) {
      reset({ descricao: categoria.descricao, finalidade: categoria.finalidade })
    }
  }, [categoria, reset])

  const mutation = useMutation({
    mutationFn: (data: CategoriaFormData) =>
      isEditing ? categoriaApi.editar(id!, data) : categoriaApi.criar(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] })
      navigate('/categorias')
    },
    onError: (err: Error) => {
      setError('root', { message: err.message })
    },
  })

  return (
    <div className="max-w-lg">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">
        {isEditing ? 'Editar Categoria' : 'Nova Categoria'}
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
            placeholder="Descrição da categoria"
          />
          {errors.descricao && (
            <p className="mt-1 text-sm text-red-600">{errors.descricao.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Finalidade</label>
          <select
            {...register('finalidade', { valueAsNumber: true })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
          >
            <option value={0}>Despesa</option>
            <option value={1}>Receita</option>
            <option value={2}>Ambas</option>
          </select>
          {errors.finalidade && (
            <p className="mt-1 text-sm text-red-600">{errors.finalidade.message}</p>
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
            onClick={() => navigate('/categorias')}
            className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 text-sm font-medium"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
