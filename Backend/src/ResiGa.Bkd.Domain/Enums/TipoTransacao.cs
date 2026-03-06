using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TipoTransacao
{
    Despesa = 0,
    Receita = 1
}
