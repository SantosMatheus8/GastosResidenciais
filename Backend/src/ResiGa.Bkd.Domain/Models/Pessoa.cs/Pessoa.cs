namespace ResiGa.Bkd.Domain.Models.Pessoa;

/// <summary>
/// Entidade Pessoa - representa um morador/participante do controle de gastos.
/// Campos:
/// - Id: identificador unico, gerado automaticamente pelo banco (GUID).
/// - Nome: nome da pessoa, maximo 200 caracteres.
/// - Idade: idade em anos. Se menor de 18, a pessoa so pode ter transacoes do tipo Despesa.
/// </summary>
public class Pessoa
{
    public Guid? Id { get; set; }
    public string Nome { get; set; } = "";
    public int Idade { get; set; }
}
