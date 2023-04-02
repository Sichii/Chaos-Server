using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a dialog template
/// </summary>
public sealed record DialogTemplateSchema
{
    /// <summary>
    ///     Whether or not this dialog requires context passed to it from the previous dialog
    /// </summary>
    public bool Contextual { get; set; }
    /// <summary>
    ///     The template key of the dialog that should be displayed after this one<br />If specified and the <see cref="Type" /> is Normal, the
    ///     dialog will have a next button
    /// </summary>
    public string? NextDialogKey { get; set; }
    /// <summary>
    ///     A collection of options that can be selected from this dialog
    /// </summary>
    public ICollection<DialogOptionSchema> Options { get; set; } = Array.Empty<DialogOptionSchema>();
    /// <summary>
    ///     If specified and the <see cref="Type" /> is Normal, the dialog will have a previous button that will take them to the dialog with this
    ///     template key
    /// </summary>
    public string? PrevDialogKey { get; set; }

    /// <summary>
    ///     A collection of names of dialog scripts to attach to this dialog by default
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of
    ///     that entry is a dictionary of propertyName-Value pairs
    /// </summary>
    public IDictionary<string, DynamicVars> ScriptVars { get; set; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     A unique id specific to this template. This must match the file name
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;

    /// <summary>
    ///     The text displayed on the template. This can be a string format with injectable parameters, but those parameters must be injected by a
    ///     script
    /// </summary>
    [JsonRequired]
    public string Text { get; set; } = null!;

    /// <summary>
    ///     When the <see cref="Type" /> is DialogTextEntry, this will limit the length of the input text box
    /// </summary>
    public ushort? TextBoxLength { get; set; }

    /// <summary>
    ///     The type of dialog this is
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public ChaosDialogType Type { get; set; }
}