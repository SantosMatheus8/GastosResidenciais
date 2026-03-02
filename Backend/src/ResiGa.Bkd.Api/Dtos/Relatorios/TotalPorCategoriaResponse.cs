namespace ResiGa.Bkd.Api.Dtos.Relatorios;

/// <summary>
/// DTO de resposta para totais financeiros de uma categoria.
/// </summary>
public class TotalPorCategoriaResponse
{
    public Guid CategoriaId { get; set; }
    public string Descricao { get; set; } = "";
    public int Finalidade { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}
