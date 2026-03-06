namespace ResiGa.Bkd.Domain.Models.Categoria;

public class Categoria
{
    public Guid? Id { get; set; }
    public string Descricao { get; set; } = "";
    public int Finalidade { get; set; }
}
