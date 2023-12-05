using System.Text.Json.Serialization;

namespace Chaos.Schemas.MetaData;

/// <summary>
///     Represents the serializable schema of the details of a mundane illustration as part of the mundane illustrations
///     meta data
/// </summary>
public sealed record MundaneIllustrationMetaSchema
{
    /// <summary>
    ///     The name of the image file to use for this merchant's illustration
    /// </summary>
    [JsonRequired]
    public string ImageName { get; set; } = null!;

    /// <summary>
    ///     The name of the merchant to show this illustration for
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;
}