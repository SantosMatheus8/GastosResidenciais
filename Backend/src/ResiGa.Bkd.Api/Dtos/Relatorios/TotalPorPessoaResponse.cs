namespace ResiGa.Bkd.Api.Dtos.Relatorios;

public class TotalPorPessoaResponse
{
    public Guid PessoaId { get; set; }
    public string Nome { get; set; } = "";
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}
