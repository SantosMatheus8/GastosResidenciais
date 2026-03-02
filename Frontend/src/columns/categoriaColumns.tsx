import { LinkButton, Button, FinalidadeBadge, type Column } from '../components/ui'
import type { Categoria } from '../types'

export function getCategoriaColumns(
  setDeleteId: (id: string) => void
): Column<Categoria>[] {
  return [
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
}
