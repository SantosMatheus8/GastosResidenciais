namespace ResiGa.Bkd.Domain.Models.Transacao;

/// <summary>
/// Entidade Transacao - representa uma movimentacao financeira (despesa ou receita).
/// Campos:
/// - Id: identificador unico, gerado automaticamente pelo banco (GUID).
/// - Descricao: descricao da transacao, maximo 400 caracteres.
/// - Valor: valor monetario da transacao, deve ser sempre positivo (maior que zero).
/// - Tipo: tipo da transacao. 0 = Despesa, 1 = Receita.
/// - CategoriaId: FK para a categoria. Deve ser compativel com o Tipo (ver regra de compatibilidade).
/// - PessoaId: FK para a pessoa associada. Se menor de 18, so aceita Despesa.
///
/// REGRAS DE NEGOCIO:
/// 1. Pessoa menor de 18 anos so pode ter transacoes do tipo Despesa.
/// 2. Categoria deve ser compativel: Despesa aceita cat. Despesa/Ambas; Receita aceita cat. Receita/Ambas.
/// </summary>
public class Transacao
{
    public Guid? Id { get; set; }
    public string Descricao { get; set; } = "";
    public decimal Valor { get; set; }
    public int Tipo { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid PessoaId { get; set; }
}
