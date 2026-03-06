using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Categoria;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Domain.Interfaces.Services;

public interface ICategoriaService
{
    Task<Categoria> CreateCategoriaAsync(Categoria categoria);
    Task<PaginatedResult<Categoria>> GetCategoriasAsync(ListCategorias listCategorias);
    Task<Categoria> FindCategoriaByIdAsync(Guid categoriaId);
    Task<Categoria> UpdateCategoriaAsync(Categoria updateCategoriaRequest, Guid categoriaId);
    Task DeleteCategoriaAsync(Guid categoriaId);
    Task<RelatorioTotais<TotalPorCategoria>> GetTotaisPorCategoriaAsync();
}
