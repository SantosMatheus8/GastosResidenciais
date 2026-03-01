using ResiGa.Bkd.Domain.Enums;

namespace ResiGa.Bkd.Domain.Models.Categoria;

public class ListCategorias
{
    public Guid? Id { get; set; }
    public string? Descricao { get; set; }
    public int? Finalidade { get; set; }
    public OrderDirection OrderDirection { get; set; }
    public CategoriaOrderBy OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 20;
}
