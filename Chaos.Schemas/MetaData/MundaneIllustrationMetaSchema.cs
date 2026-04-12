#region
using System.Text.Json.Serialization;
#endregion

namespace Chaos.Schemas.MetaData;

/// <summary>
///     Represents the serializable schema of the details of a mundane illustration as part of the mundane illustrations
///     meta data
/// </summary>
public sealed record MundaneIllustrationMetaSchema
{
    /// <summary>
    ///     One or more SPF filenames to register as illustration variants for this merchant. On the client, these are
    ///     appended to the merchant's variant list after any entries present in <c>npci.tbl</c>, and a dialog's
    ///     <c>IllustrationIndex</c> picks one by position. Almost every merchant only needs a single entry here —
    ///     multi-variant NPCs are rare.
    /// </summary>
    [JsonRequired]
    public ICollection<string> ImageNames { get; set; } = [];

    /// <summary>
    ///     The name of the merchant to show this illustration for
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;
}