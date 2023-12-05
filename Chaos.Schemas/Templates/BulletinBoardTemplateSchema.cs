using System.Text.Json.Serialization;
using Chaos.Collections.Common;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a bulletin board template
/// </summary>
public sealed record BulletinBoardTemplateSchema
{
    /// <summary>
    ///     The name of the board, as displayed if shown in a list of boards
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     A collection of names of dialog scripts to attach to this dialog by default
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value
    ///     pair, and the value of
    ///     that entry is a dictionary of propertyName-Value pairs
    /// </summary>
    public IDictionary<string, DynamicVars> ScriptVars { get; set; }
        = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     A unique id specific to this template. This must match the file name
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
}