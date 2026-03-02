import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'
import { PageHeader, LinkButton, Button, DataTable, ErrorAlert, type Column } from '../../components/ui'
import ConfirmDialog from '../../components/ConfirmDialog'
import type { Pessoa } from '../../types'

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

  // Definição declarativa das colunas da tabela
  const columns: Column<Pessoa>[] = [
    { header: 'Nome', accessor: (p) => p.nome, cellClassName: 'text-gray-900' },
    { header: 'Idade', accessor: (p) => p.idade, cellClassName: 'text-gray-600' },
    {
      header: 'Ações',
      headerClassName: 'text-right',
      cellClassName: 'text-right space-x-2',
      accessor: (p) => (
        <>
          <LinkButton to={`/pessoas/${p.id}/editar`} variant="ghost-primary">Editar</LinkButton>
          <Button variant="ghost-danger" onClick={() => setDeleteId(p.id)}>Excluir</Button>
        </>
      ),
    },
  ]

  return (
    <div>
      <PageHeader title="Pessoas">
        <LinkButton to="/pessoas/novo">Nova Pessoa</LinkButton>
      </PageHeader>

      <ErrorAlert message={erro} />

      <DataTable
        columns={columns}
        data={data?.lines ?? []}
        keyExtractor={(p) => p.id}
        isLoading={isLoading}
        emptyMessage="Nenhuma pessoa cadastrada."
      />

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
