namespace ResiGa.Bkd.Api.Dtos.Relatorios;

/// <summary>
/// DTO generico de resposta para relatorios de totais.
/// Contém a lista de itens e os totais gerais consolidados.
/// </summary>
public class RelatorioTotaisResponse<T>
{
    /// <summary>
    /// Lista de itens com totais individuais (por pessoa ou por categoria).
    /// </summary>
    public List<T> Itens { get; set; } = [];

    /// <summary>
    /// Soma total de todas as receitas.
    /// </summary>
    public decimal TotalGeralReceitas { get; set; }

    /// <summary>
    /// Soma total de todas as despesas.
    /// </summary>
    public decimal TotalGeralDespesas { get; set; }

    /// <summary>
    /// Saldo liquido geral (TotalGeralReceitas - TotalGeralDespesas).
    /// </summary>
    public decimal SaldoLiquido { get; set; }
}
