using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Finalidade
{
    Despesa = 0,
    Receita = 1,
    Ambas = 2
}
