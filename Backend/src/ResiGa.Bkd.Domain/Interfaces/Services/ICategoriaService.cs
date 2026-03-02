using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Categoria;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Domain.Interfaces.Services;

/// <summary>
/// Interface do servico de Categoria.
/// Define o contrato de regras de negocio para a entidade Categoria.
/// </summary>
public interface ICategoriaService
{
    Task<Categoria> CreateCategoriaAsync(Categoria categoria);
    Task<PaginatedResult<Categoria>> GetCategoriasAsync(ListCategorias listCategorias);
    Task<Categoria> FindCategoriaByIdAsync(Guid categoriaId);
    Task<Categoria> UpdateCategoriaAsync(Categoria updateCategoriaRequest, Guid categoriaId);
    Task DeleteCategoriaAsync(Guid categoriaId);

    /// <summary>
    /// Retorna relatorio com totais de receitas, despesas e saldo por categoria,
    /// alem dos totais gerais somados de todas as categorias.
    /// </summary>
    Task<RelatorioTotais<TotalPorCategoria>> GetTotaisPorCategoriaAsync();
}
