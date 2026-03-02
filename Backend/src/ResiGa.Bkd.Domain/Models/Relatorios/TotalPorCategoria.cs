namespace ResiGa.Bkd.Domain.Models.Relatorios;

/// <summary>
/// Modelo que representa os totais financeiros agrupados por categoria.
/// Contém o total de receitas, despesas e o saldo (receitas - despesas).
/// </summary>
public class TotalPorCategoria
{
    public Guid CategoriaId { get; set; }
    public string Descricao { get; set; } = "";
    public int Finalidade { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }

    /// <summary>
    /// Saldo = TotalReceitas - TotalDespesas.
    /// Valor positivo indica superavit, negativo indica deficit.
    /// </summary>
    public decimal Saldo { get; set; }
}
