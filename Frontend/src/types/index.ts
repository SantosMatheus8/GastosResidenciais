export enum Finalidade {
  Despesa = 0,
  Receita = 1,
  Ambas = 2,
}

export enum TipoTransacao {
  Despesa = 0,
  Receita = 1,
}

export const FinalidadeLabel: Record<number, string> = {
  [Finalidade.Despesa]: 'Despesa',
  [Finalidade.Receita]: 'Receita',
  [Finalidade.Ambas]: 'Ambas',
}

export const TipoTransacaoLabel: Record<number, string> = {
  [TipoTransacao.Despesa]: 'Despesa',
  [TipoTransacao.Receita]: 'Receita',
}

export interface Pessoa {
  id: string
  nome: string
  idade: number
}

export interface Categoria {
  id: string
  descricao: string
  finalidade: number
}

export interface Transacao {
  id: string
  descricao: string
  valor: number
  tipo: number
  categoriaId: string
  pessoaId: string
}

export interface PaginatedResult<T> {
  lines: T[]
  page: number
  pageSize: number
  hasPreviousPage: boolean
  hasNextPage: boolean
  totalItens: number
  totalPages: number
}

export interface TotalPorPessoa {
  pessoaId: string
  nome: string
  totalReceitas: number
  totalDespesas: number
  saldo: number
}

export interface TotalPorCategoria {
  categoriaId: string
  descricao: string
  finalidade: number
  totalReceitas: number
  totalDespesas: number
  saldo: number
}

export interface RelatorioTotais<T> {
  itens: T[]
  totalGeralReceitas: number
  totalGeralDespesas: number
  saldoLiquido: number
}
