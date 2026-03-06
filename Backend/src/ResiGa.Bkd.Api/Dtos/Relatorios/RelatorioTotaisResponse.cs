namespace ResiGa.Bkd.Api.Dtos.Relatorios;

public class RelatorioTotaisResponse<T>
{
    public List<T> Itens { get; set; } = [];
    public decimal TotalGeralReceitas { get; set; }
    public decimal TotalGeralDespesas { get; set; }
    public decimal SaldoLiquido { get; set; }
}
