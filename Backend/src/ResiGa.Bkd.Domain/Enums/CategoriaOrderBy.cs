using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoriaOrderBy
{
    [Description("c.Id")]
    Id,
    [Description("c.Descricao")]
    Descricao,
    [Description("c.Finalidade")]
    Finalidade,
}
