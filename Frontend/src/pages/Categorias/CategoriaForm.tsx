import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriaApi } from '../../services/api'
import { PageHeader, Input, Select, Button, ErrorAlert } from '../../components/ui'

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
    if (categoria) reset({ descricao: categoria.descricao, finalidade: categoria.finalidade })
  }, [categoria, reset])

  const mutation = useMutation({
    mutationFn: (data: CategoriaFormData) =>
      isEditing ? categoriaApi.editar(id!, data) : categoriaApi.criar(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] })
      navigate('/categorias')
    },
    onError: (err: Error) => setError('root', { message: err.message }),
  })

  return (
    <div className="max-w-lg">
      <PageHeader title={isEditing ? 'Editar Categoria' : 'Nova Categoria'} />

      <form
        onSubmit={handleSubmit((data) => mutation.mutate(data))}
        className="bg-white rounded-lg shadow p-6 space-y-4"
      >
        <ErrorAlert message={errors.root?.message} />

        <Input
          label="Descrição"
          placeholder="Descrição da categoria"
          error={errors.descricao?.message}
          {...register('descricao')}
        />

        <Select
          label="Finalidade"
          error={errors.finalidade?.message}
          {...register('finalidade', { valueAsNumber: true })}
        >
          <option value={0}>Despesa</option>
          <option value={1}>Receita</option>
          <option value={2}>Ambas</option>
        </Select>

        <div className="flex gap-3 pt-2">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Salvando...' : 'Salvar'}
          </Button>
          <Button variant="secondary" type="button" onClick={() => navigate('/categorias')}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  )
}
