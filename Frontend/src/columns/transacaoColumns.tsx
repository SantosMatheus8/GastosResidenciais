import { LinkButton, Button, TipoBadge, type Column } from '../components/ui'
import { formatCurrency } from '../utils/format'
import type { Transacao } from '../types'

export function getTransacaoColumns(
  pessoaMap: Map<string, string>,
  categoriaMap: Map<string, string>,
  setDeleteId: (id: string) => void
): Column<Transacao>[] {
  return [
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
}
