import { type ReactNode } from 'react'

// Definição de coluna para o DataTable.
// cellClassName pode ser string fixa ou função que recebe a linha
// para estilização dinâmica (ex: cor do saldo baseada no valor).
export interface Column<T> {
  header: string
  accessor: (row: T) => ReactNode
  headerClassName?: string
  cellClassName?: string | ((row: T) => string)
}

interface DataTableProps<T> {
  columns: Column<T>[]
  data: T[]
  keyExtractor: (row: T) => string
  emptyMessage?: string
  isLoading?: boolean
  footer?: ReactNode
}

// Tabela de dados genérica e reutilizável.
// Renderiza cabeçalho, linhas de dados, estado vazio e rodapé opcional.
export default function DataTable<T>({
  columns,
  data,
  keyExtractor,
  emptyMessage = 'Nenhum registro encontrado.',
  isLoading,
  footer,
}: DataTableProps<T>) {
  if (isLoading) return <p className="text-gray-500">Carregando...</p>

  return (
    <div className="bg-white rounded-lg shadow overflow-x-auto">
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-50">
          <tr>
            {columns.map((col) => (
              <th
                key={col.header}
                className={`px-6 py-3 text-xs font-medium text-gray-500 uppercase ${
                  col.headerClassName ?? 'text-left'
                }`}
              >
                {col.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200">
          {data.map((row) => (
            <tr key={keyExtractor(row)} className="hover:bg-gray-50">
              {columns.map((col) => {
                const cls =
                  typeof col.cellClassName === 'function'
                    ? col.cellClassName(row)
                    : (col.cellClassName ?? '')
                return (
                  <td key={col.header} className={`px-6 py-4 text-sm ${cls}`}>
                    {col.accessor(row)}
                  </td>
                )
              })}
            </tr>
          ))}

          {data.length === 0 && (
            <tr>
              <td
                colSpan={columns.length}
                className="px-6 py-8 text-center text-gray-500"
              >
                {emptyMessage}
              </td>
            </tr>
          )}
          {footer}
        </tbody>
      </table>
    </div>
  )
}
