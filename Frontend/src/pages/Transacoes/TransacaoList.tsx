import { useState, useMemo } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { transacaoApi, pessoaApi, categoriaApi } from '../../services/api'
import { PageHeader, LinkButton, DataTable, ErrorAlert } from '../../components/ui'
import { getTransacaoColumns } from '../../columns'
import ConfirmDialog from '../../components/ConfirmDialog'

export default function TransacaoList() {
  const queryClient = useQueryClient()
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [erro, setErro] = useState('')

  const { data: transacoes, isLoading } = useQuery({
    queryKey: ['transacoes'],
    queryFn: () => transacaoApi.listar(),
  })

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

  const pessoaMap = useMemo(
    () => new Map(pessoas?.lines.map((p) => [p.id, p.nome]) ?? []),
    [pessoas]
  )
  const categoriaMap = useMemo(
    () => new Map(categorias?.lines.map((c) => [c.id, c.descricao]) ?? []),
    [categorias]
  )

  const columns = useMemo(
    () => getTransacaoColumns(pessoaMap, categoriaMap, setDeleteId),
    [pessoaMap, categoriaMap]
  )

  return (
    <div>
      <PageHeader title="Transações">
        <LinkButton to="/transacoes/novo">Nova Transação</LinkButton>
      </PageHeader>

      <ErrorAlert message={erro} />

      <DataTable
        columns={columns}
        data={transacoes?.lines ?? []}
        keyExtractor={(t) => t.id}
        isLoading={isLoading}
        emptyMessage="Nenhuma transação cadastrada."
      />

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
