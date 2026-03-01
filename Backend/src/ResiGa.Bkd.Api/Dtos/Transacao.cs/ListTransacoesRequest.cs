using ResiGa.Bkd.Domain.Enums;

namespace ResiGa.Bkd.Api.Dtos.Transacao;

public class ListTransacoesRequest
{
    public Guid? Id { get; set; }
    public string? Descricao { get; set; }
    public int? Tipo { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? PessoaId { get; set; }
    public OrderDirection OrderDirection { get; set; }
    public TransacaoOrderBy OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 20;
}
