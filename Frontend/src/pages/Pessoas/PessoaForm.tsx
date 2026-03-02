import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'
import { pessoaSchema, type PessoaFormData } from '../../schemas'
import { PageHeader, Input, Button, ErrorAlert } from '../../components/ui'

export default function PessoaForm() {
  const { id } = useParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEditing = Boolean(id)

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<PessoaFormData>({
    resolver: zodResolver(pessoaSchema),
    defaultValues: { nome: '', idade: 0 },
  })

  // Carrega dados existentes ao editar
  const { data: pessoa } = useQuery({
    queryKey: ['pessoa', id],
    queryFn: () => pessoaApi.buscar(id!),
    enabled: isEditing,
  })

  // Preenche os campos com os dados carregados
  useEffect(() => {
    if (pessoa) {
      setValue('nome', pessoa.nome)
      setValue('idade', pessoa.idade)
    }
  }, [pessoa, setValue])

  const mutation = useMutation({
    mutationFn: (data: PessoaFormData) =>
      isEditing ? pessoaApi.editar(id!, data) : pessoaApi.criar(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pessoas'] })
      navigate('/pessoas')
    },
    onError: (err: Error) => setError('root', { message: err.message }),
  })

  return (
    <div className="max-w-lg">
      <PageHeader title={isEditing ? 'Editar Pessoa' : 'Nova Pessoa'} />

      <form
        onSubmit={handleSubmit((data) => mutation.mutate(data))}
        className="bg-white rounded-lg shadow p-6 space-y-4"
      >
        <ErrorAlert message={errors.root?.message} />

        <Input
          label="Nome"
          placeholder="Nome da pessoa"
          error={errors.nome?.message}
          {...register('nome')}
        />

        <Input
          label="Idade"
          type="number"
          placeholder="0"
          error={errors.idade?.message}
          {...register('idade', { valueAsNumber: true })}
        />

        <div className="flex gap-3 pt-2">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Salvando...' : 'Salvar'}
          </Button>
          <Button variant="secondary" type="button" onClick={() => navigate('/pessoas')}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  )
}
