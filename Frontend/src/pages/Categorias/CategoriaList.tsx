import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriaApi } from '../../services/api'
import { PageHeader, LinkButton, Button, DataTable, ErrorAlert, FinalidadeBadge, type Column } from '../../components/ui'
import ConfirmDialog from '../../components/ConfirmDialog'
import type { Categoria } from '../../types'

export default function CategoriaList() {
  const queryClient = useQueryClient()
  const [deleteId, setDeleteId] = useState<string | null>(null)
  const [erro, setErro] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['categorias'],
    queryFn: () => categoriaApi.listar(),
  })

  const deleteMutation = useMutation({
    mutationFn: categoriaApi.deletar,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] })
      setDeleteId(null)
      setErro('')
    },
    onError: (err: Error) => {
      setErro(err.message)
      setDeleteId(null)
    },
  })

  const columns: Column<Categoria>[] = [
    { header: 'Descrição', accessor: (c) => c.descricao, cellClassName: 'text-gray-900' },
    { header: 'Finalidade', accessor: (c) => <FinalidadeBadge value={c.finalidade} /> },
    {
      header: 'Ações',
      headerClassName: 'text-right',
      cellClassName: 'text-right space-x-2',
      accessor: (c) => (
        <>
          <LinkButton to={`/categorias/${c.id}/editar`} variant="ghost-primary">Editar</LinkButton>
          <Button variant="ghost-danger" onClick={() => setDeleteId(c.id)}>Excluir</Button>
        </>
      ),
    },
  ]

  return (
    <div>
      <PageHeader title="Categorias">
        <LinkButton to="/categorias/novo">Nova Categoria</LinkButton>
      </PageHeader>

      <ErrorAlert message={erro} />

      <DataTable
        columns={columns}
        data={data?.lines ?? []}
        keyExtractor={(c) => c.id}
        isLoading={isLoading}
        emptyMessage="Nenhuma categoria cadastrada."
      />

      <ConfirmDialog
        open={deleteId !== null}
        title="Excluir Categoria"
        message="Tem certeza que deseja excluir esta categoria?"
        onConfirm={() => deleteId && deleteMutation.mutate(deleteId)}
        onCancel={() => setDeleteId(null)}
      />
    </div>
  )
}
