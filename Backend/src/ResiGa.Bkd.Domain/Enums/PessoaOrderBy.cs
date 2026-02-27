using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PessoaOrderBy
{
    [Description("p.Id")]
    Id,
    [Description("p.Nome")]
    Nome,
    [Description("p.Idade")]
    Idade,
}

