#region
using System.Text.Json.Serialization;
#endregion

namespace Chaos.Schemas.Aisling.Abstractions;

/// <summary>
///     Represents the serializable schema of a PanelEntity
/// </summary>
public abstract record PanelEntitySchema
{
    /// <summary>
    ///     The amount of time in milliseconds that has elapsed towards the cooldown of this object
    /// </summary>
    public int? ElapsedMs { get; set; }

    /// <summary>
    ///     A collection of names of scripts to attach to this object by default
    /// </summary>
    /// ]
    public ICollection<string>? ScriptKeys { get; set; } = [];

    /// <summary>
    ///     The slot this object is in
    /// </summary>
    public byte? Slot { get; set; }

    /// <summary>
    ///     A unique id specific to this template. This must match the file name
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;

    /// <summary>
    ///     A unique id specific to this an instance of this object. Used for tracking purposes
    /// </summary>
    public ulong UniqueId { get; set; }
}