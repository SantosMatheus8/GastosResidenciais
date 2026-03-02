using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

/// <summary>
/// Enum que representa o tipo de uma transacao financeira.
/// Despesa (0) = saida de dinheiro, Receita (1) = entrada de dinheiro.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TipoTransacao
{
    Despesa = 0,
    Receita = 1
}
