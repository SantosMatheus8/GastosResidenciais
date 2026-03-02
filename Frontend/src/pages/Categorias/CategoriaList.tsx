import { useState, useMemo } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriaApi } from '../../services/api'
import { PageHeader, LinkButton, DataTable, ErrorAlert } from '../../components/ui'
import { getCategoriaColumns } from '../../columns'
import ConfirmDialog from '../../components/ConfirmDialog'

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

  const columns = useMemo(() => getCategoriaColumns(setDeleteId), [])

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
