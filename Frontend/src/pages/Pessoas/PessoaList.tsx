import { useState, useMemo } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pessoaApi } from '../../services/api'
import { PageHeader, LinkButton, DataTable, ErrorAlert } from '../../components/ui'
import { getPessoaColumns } from '../../columns'
import ConfirmDialog from '../../components/ConfirmDialog'

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

  const columns = useMemo(() => getPessoaColumns(setDeleteId), [])

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
