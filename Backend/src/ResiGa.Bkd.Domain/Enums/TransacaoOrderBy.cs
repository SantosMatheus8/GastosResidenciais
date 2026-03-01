using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransacaoOrderBy
{
    [Description("t.Id")]
    Id,
    [Description("t.Descricao")]
    Descricao,
    [Description("t.Valor")]
    Valor,
    [Description("t.Tipo")]
    Tipo,
}
