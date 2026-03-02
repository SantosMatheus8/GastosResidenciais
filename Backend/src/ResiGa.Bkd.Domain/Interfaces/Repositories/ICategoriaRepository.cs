using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Categoria;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Domain.Interfaces.Repositories;

/// <summary>
/// Interface do repositorio de Categoria.
/// Define o contrato de acesso a dados para operacoes com a entidade Categoria.
/// </summary>
public interface ICategoriaRepository
{
    Task<Categoria> CreateCategoriaAsync(Categoria categoria);
    Task<PaginatedResult<Categoria>> GetCategoriasAsync(ListCategorias listCategorias);
    Task<Categoria?> FindCategoriaByIdAsync(Guid categoriaId);
    Task UpdateCategoriaAsync(Categoria categoria);
    Task DeleteCategoriaAsync(Guid categoriaId);

    /// <summary>
    /// Retorna os totais financeiros (receitas, despesas, saldo) agrupados por categoria.
    /// </summary>
    Task<List<TotalPorCategoria>> GetTotaisPorCategoriaAsync();
}
