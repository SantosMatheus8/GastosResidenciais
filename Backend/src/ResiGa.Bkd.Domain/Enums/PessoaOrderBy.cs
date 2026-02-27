using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PessoaOrderBy
{
    [Description("b.Id")]
    Id,
    [Description("b.Nome")]
    Nome,
    [Description("b.Idade")]
    Idade,
}

