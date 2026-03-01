using ResiGa.Bkd.Domain.Enums;

namespace ResiGa.Bkd.Api.Dtos.Categoria;

public class ListCategoriasRequest
{
    public Guid? Id { get; set; }
    public string? Descricao { get; set; }
    public int? Finalidade { get; set; }
    public OrderDirection OrderDirection { get; set; }
    public CategoriaOrderBy OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 20;
}
