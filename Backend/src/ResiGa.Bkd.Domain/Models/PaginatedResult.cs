namespace ResiGa.Bkd.Domain.Models;

/// <summary>
/// Modelo generico de resultado paginado.
/// Encapsula a lista de itens da pagina atual junto com metadados de paginacao.
/// </summary>
public class PaginatedResult<T>
{
    /// <summary>
    /// Itens da pagina atual.
    /// </summary>
    public List<T> Lines { get; set; } = [];

    /// <summary>
    /// Numero da pagina atual (comeca em 1).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Quantidade de itens por pagina.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Indica se existe pagina anterior.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indica se existe proxima pagina.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Total de itens em todas as paginas.
    /// </summary>
    public int TotalItens { get; set; }

    /// <summary>
    /// Total de paginas disponiveis.
    /// </summary>
    public int TotalPages { get; set; }
}
