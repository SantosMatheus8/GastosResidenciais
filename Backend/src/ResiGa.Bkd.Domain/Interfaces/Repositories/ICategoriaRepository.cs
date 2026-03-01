using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Categoria;

namespace ResiGa.Bkd.Domain.Interfaces.Repositories;

public interface ICategoriaRepository
{
    Task<Categoria> CreateCategoriaAsync(Categoria categoria);
    Task<PaginatedResult<Categoria>> GetCategoriasAsync(ListCategorias listCategorias);
    Task<Categoria?> FindCategoriaByIdAsync(Guid categoriaId);
    Task UpdateCategoriaAsync(Categoria categoria);
    Task DeleteCategoriaAsync(Guid categoriaId);
}
