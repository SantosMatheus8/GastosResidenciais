namespace ResiGa.Bkd.Domain.Models.Relatorios;

/// <summary>
/// Modelo que representa os totais financeiros agrupados por pessoa.
/// Contém o total de receitas, despesas e o saldo (receitas - despesas).
/// </summary>
public class TotalPorPessoa
{
    public Guid PessoaId { get; set; }
    public string Nome { get; set; } = "";
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }

    /// <summary>
    /// Saldo = TotalReceitas - TotalDespesas.
    /// Valor positivo indica superavit, negativo indica deficit.
    /// </summary>
    public decimal Saldo { get; set; }
}
