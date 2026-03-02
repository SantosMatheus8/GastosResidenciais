import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { transacaoApi, pessoaApi, categoriaApi } from '../../services/api'
import { formatCurrency } from '../../utils/format'
import { PageHeader, LinkButton, Button, DataTable, ErrorAlert, TipoBadge, type Column } from '../../components/ui'
import ConfirmDialog from '../../components/ConfirmDialog'
import type { Transacao } from '../../types'

export default function TransacaoList() {
  const queryClient = useQueryClient()
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [erro, setErro] = useState('')

  const { data: transacoes, isLoading } = useQuery({
    queryKey: ['transacoes'],
    queryFn: () => transacaoApi.listar(),
  })

  // Busca pessoas e categorias para resolver nomes na tabela
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

  // Mapas para lookup rápido de nomes por ID
  const pessoaMap = new Map(pessoas?.lines.map((p) => [p.id, p.nome]) ?? [])
  const categoriaMap = new Map(categorias?.lines.map((c) => [c.id, c.descricao]) ?? [])

  const columns: Column<Transacao>[] = [
    { header: 'Descrição', accessor: (t) => t.descricao, cellClassName: 'text-gray-900' },
    { header: 'Pessoa', accessor: (t) => pessoaMap.get(t.pessoaId) ?? '—', cellClassName: 'text-gray-600' },
    { header: 'Categoria', accessor: (t) => categoriaMap.get(t.categoriaId) ?? '—', cellClassName: 'text-gray-600' },
    { header: 'Tipo', accessor: (t) => <TipoBadge value={t.tipo} /> },
    {
      header: 'Valor',
      headerClassName: 'text-right',
      cellClassName: 'text-right font-medium',
      accessor: (t) => formatCurrency(t.valor),
    },
    {
      header: 'Ações',
      headerClassName: 'text-right',
      cellClassName: 'text-right space-x-2',
      accessor: (t) => (
        <>
          <LinkButton to={`/transacoes/${t.id}/editar`} variant="ghost-primary">Editar</LinkButton>
          <Button variant="ghost-danger" onClick={() => setDeleteId(t.id)}>Excluir</Button>
        </>
      ),
    },
  ]

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
