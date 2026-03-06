import { LinkButton, Button, type Column } from '../components/ui'
import type { Pessoa } from '../types'

export function getPessoaColumns(
  setDeleteId: (id: string) => void
): Column<Pessoa>[] {
  return [
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
}
