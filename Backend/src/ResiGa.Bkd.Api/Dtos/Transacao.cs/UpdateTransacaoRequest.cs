namespace ResiGa.Bkd.Api.Dtos.Transacao;

public class UpdateTransacaoRequest
{
    public string Descricao { get; set; } = "";
    public decimal Valor { get; set; }
    public int Tipo { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid PessoaId { get; set; }
}
