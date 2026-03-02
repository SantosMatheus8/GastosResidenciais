using System.ComponentModel;
using System.Reflection;

namespace ResiGa.Bkd.Domain.Utils;

/// <summary>
/// Extensoes para enums.
/// Permite extrair o valor do atributo [Description] de um enum,
/// usado para mapear enums de ordenacao para nomes de colunas SQL.
/// Exemplo: PessoaOrderBy.Nome tem [Description("b.Nome")] que retorna "b.Nome".
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Retorna o valor do atributo [Description] do enum, ou o nome do enum se nao tiver atributo.
    /// </summary>
    public static string GetEnumDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}
