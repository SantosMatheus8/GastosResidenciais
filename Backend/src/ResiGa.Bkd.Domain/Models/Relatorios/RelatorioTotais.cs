namespace ResiGa.Bkd.Domain.Models.Relatorios;

/// <summary>
/// Modelo generico de relatorio de totais.
/// Contém a lista de itens agrupados e os totais gerais somados.
/// </summary>
public class RelatorioTotais<T>
{
    /// <summary>
    /// Lista de itens com totais individuais (por pessoa ou por categoria).
    /// </summary>
    public List<T> Itens { get; set; } = [];

    /// <summary>
    /// Soma total de todas as receitas de todos os itens.
    /// </summary>
    public decimal TotalGeralReceitas { get; set; }

    /// <summary>
    /// Soma total de todas as despesas de todos os itens.
    /// </summary>
    public decimal TotalGeralDespesas { get; set; }

    /// <summary>
    /// Saldo liquido geral = TotalGeralReceitas - TotalGeralDespesas.
    /// </summary>
    public decimal SaldoLiquido { get; set; }
}
