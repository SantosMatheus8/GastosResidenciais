import type {
  Pessoa,
  Categoria,
  Transacao,
  PaginatedResult,
  RelatorioTotais,
  TotalPorPessoa,
  TotalPorCategoria,
} from '../types'

// URL base da API — vazio usa proxy (Vite em dev, nginx em Docker)
const BASE_URL = import.meta.env.VITE_API_URL || ''

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })

  if (!res.ok) {
    const body = await res.text()
    throw new Error(body || `Erro ${res.status}`)
  }

  // DELETE pode retornar 204 sem body
  if (res.status === 204) return undefined as T

  return res.json()
}

// === Pessoa ===

export const pessoaApi = {
  listar: (page = 1, itemsPerPage = 50) =>
    request<PaginatedResult<Pessoa>>(
      `/v1/pessoa?Page=${page}&ItemsPerPage=${itemsPerPage}`
    ),

  buscar: (id: string) => request<Pessoa>(`/v1/pessoa/${id}`),

  criar: (data: { nome: string; idade: number }) =>
    request<Pessoa>('/v1/pessoa', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  editar: (id: string, data: { nome: string; idade: number }) =>
    request<Pessoa>(`/v1/pessoa/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),

  deletar: (id: string) =>
    request<void>(`/v1/pessoa/${id}`, { method: 'DELETE' }),

  totais: () =>
    request<RelatorioTotais<TotalPorPessoa>>('/v1/pessoa/totais'),
}

// === Categoria ===

export const categoriaApi = {
  listar: (page = 1, itemsPerPage = 50) =>
    request<PaginatedResult<Categoria>>(
      `/v1/categoria?Page=${page}&ItemsPerPage=${itemsPerPage}`
    ),

  buscar: (id: string) => request<Categoria>(`/v1/categoria/${id}`),

  criar: (data: { descricao: string; finalidade: number }) =>
    request<Categoria>('/v1/categoria', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  editar: (id: string, data: { descricao: string; finalidade: number }) =>
    request<Categoria>(`/v1/categoria/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),

  deletar: (id: string) =>
    request<void>(`/v1/categoria/${id}`, { method: 'DELETE' }),

  totais: () =>
    request<RelatorioTotais<TotalPorCategoria>>('/v1/categoria/totais'),
}

// === Transação ===

export const transacaoApi = {
  listar: (page = 1, itemsPerPage = 50) =>
    request<PaginatedResult<Transacao>>(
      `/v1/transacao?Page=${page}&ItemsPerPage=${itemsPerPage}`
    ),

  buscar: (id: string) => request<Transacao>(`/v1/transacao/${id}`),

  criar: (data: {
    descricao: string
    valor: number
    tipo: number
    categoriaId: string
    pessoaId: string
  }) =>
    request<Transacao>('/v1/transacao', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  editar: (
    id: string,
    data: {
      descricao: string
      valor: number
      tipo: number
      categoriaId: string
      pessoaId: string
    }
  ) =>
    request<Transacao>(`/v1/transacao/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),

  deletar: (id: string) =>
    request<void>(`/v1/transacao/${id}`, { method: 'DELETE' }),
}
