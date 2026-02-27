using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderDirection
{
    [Description("ASC")]
    ASC,
    [Description("DESC")]
    DESC
}

