namespace ResiGa.Bkd.Domain.Models.Categoria;

/// <summary>
/// Entidade Categoria - classifica as transacoes financeiras.
/// Campos:
/// - Id: identificador unico, gerado automaticamente pelo banco (GUID).
/// - Descricao: descricao da categoria, maximo 400 caracteres.
/// - Finalidade: define para qual tipo de transacao a categoria pode ser usada.
///   Valores: 0 = Despesa, 1 = Receita, 2 = Ambas.
///   REGRA: ao criar transacao, a finalidade da categoria deve ser compativel com o tipo.
/// </summary>
public class Categoria
{
    public Guid? Id { get; set; }
    public string Descricao { get; set; } = "";
    public int Finalidade { get; set; }
}
