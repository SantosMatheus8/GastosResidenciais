using ResiGa.Bkd.Domain.Enums;

namespace ResiGa.Bkd.Domain.Models.Pessoa;

public class ListPessoas
{
    public Guid? Id { get; set; }
    public string? Nome { get; set; }
    public int? Idade { get; set; }
    public OrderDirection OrderDirection { get; set; }
    public PessoaOrderBy OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 20;
}