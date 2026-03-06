namespace ResiGa.Bkd.Domain.Models.Relatorios;

public class TotalPorCategoria
{
    public Guid CategoriaId { get; set; }
    public string Descricao { get; set; } = "";
    public int Finalidade { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}
