namespace ResiGa.Bkd.Domain.Models.Relatorios;

public class RelatorioTotais<T>
{
    public List<T> Itens { get; set; } = [];
    public decimal TotalGeralReceitas { get; set; }
    public decimal TotalGeralDespesas { get; set; }
    public decimal SaldoLiquido { get; set; }
}
