using System.Text.Json.Serialization;

namespace ResiGa.Bkd.Domain.Enums;

/// <summary>
/// Enum que representa a finalidade de uma categoria.
/// Despesa (0) = categoria apenas para despesas.
/// Receita (1) = categoria apenas para receitas.
/// Ambas (2) = categoria aceita tanto despesas quanto receitas.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Finalidade
{
    Despesa = 0,
    Receita = 1,
    Ambas = 2
}
