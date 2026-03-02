import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'

// Validação Zod para Pessoa
const pessoaSchema = z.object({
  nome: z
    .string()
    .min(1, 'Nome é obrigatório')
    .max(200, 'Nome deve ter no máximo 200 caracteres'),
  idade: z
    .number({ invalid_type_error: 'Idade deve ser um número' })
    .int('Idade deve ser inteira')
    .min(0, 'Idade deve ser maior ou igual a 0'),
})

type PessoaFormData = z.infer<typeof pessoaSchema>

export default function PessoaForm() {
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
  } = useForm<PessoaFormData>({
    resolver: zodResolver(pessoaSchema),
    defaultValues: { nome: '', idade: 0 },
  })

  // Carrega dados para edição
  const { data: pessoa } = useQuery({
    queryKey: ['pessoa', id],
    queryFn: () => pessoaApi.buscar(id!),
    enabled: isEditing,
  })

  useEffect(() => {
    if (pessoa) {
      reset({ nome: pessoa.nome, idade: pessoa.idade })
    }
  }, [pessoa, reset])

  const mutation = useMutation({
    mutationFn: (data: PessoaFormData) =>
      isEditing ? pessoaApi.editar(id!, data) : pessoaApi.criar(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pessoas'] })
      navigate('/pessoas')
    },
    onError: (err: Error) => {
      setError('root', { message: err.message })
    },
  })

  return (
    <div className="max-w-lg">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">
        {isEditing ? 'Editar Pessoa' : 'Nova Pessoa'}
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
          <label className="block text-sm font-medium text-gray-700 mb-1">Nome</label>
          <input
            {...register('nome')}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
            placeholder="Nome da pessoa"
          />
          {errors.nome && (
            <p className="mt-1 text-sm text-red-600">{errors.nome.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Idade</label>
          <input
            type="number"
            {...register('idade', { valueAsNumber: true })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
            placeholder="0"
          />
          {errors.idade && (
            <p className="mt-1 text-sm text-red-600">{errors.idade.message}</p>
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
            onClick={() => navigate('/pessoas')}
            className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 text-sm font-medium"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
