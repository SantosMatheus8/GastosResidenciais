import { useEffect, useMemo } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { transacaoApi, pessoaApi, categoriaApi } from '../../services/api'
import { Finalidade, TipoTransacao } from '../../types'
import { PageHeader, Input, Select, Button, ErrorAlert } from '../../components/ui'

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
    defaultValues: { descricao: '', valor: 0, tipo: 0, categoriaId: '', pessoaId: '' },
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
    onError: (err: Error) => setError('root', { message: err.message }),
  })

  return (
    <div className="max-w-lg">
      <PageHeader title={isEditing ? 'Editar Transação' : 'Nova Transação'} />

      <form
        onSubmit={handleSubmit((data) => mutation.mutate(data))}
        className="bg-white rounded-lg shadow p-6 space-y-4"
      >
        <ErrorAlert message={errors.root?.message} />

        <Input
          label="Descrição"
          placeholder="Descrição da transação"
          error={errors.descricao?.message}
          {...register('descricao')}
        />

        <Input
          label="Valor (R$)"
          type="number"
          step="0.01"
          placeholder="0,00"
          error={errors.valor?.message}
          {...register('valor', { valueAsNumber: true })}
        />

        <Select label="Tipo" {...register('tipo', { valueAsNumber: true })}>
          <option value={0}>Despesa</option>
          <option value={1}>Receita</option>
        </Select>

        <Select label="Categoria" error={errors.categoriaId?.message} {...register('categoriaId')}>
          <option value="">Selecione uma categoria</option>
          {categoriasFiltradas.map((c) => (
            <option key={c.id} value={c.id}>{c.descricao}</option>
          ))}
        </Select>

        <Select label="Pessoa" error={errors.pessoaId?.message} {...register('pessoaId')}>
          <option value="">Selecione uma pessoa</option>
          {pessoas?.lines.map((p) => (
            <option key={p.id} value={p.id}>{p.nome}</option>
          ))}
        </Select>

        <div className="flex gap-3 pt-2">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Salvando...' : 'Salvar'}
          </Button>
          <Button variant="secondary" type="button" onClick={() => navigate('/transacoes')}>
            Cancelar
          </Button>
        </div>
      </form>
    </div>
  )
}
