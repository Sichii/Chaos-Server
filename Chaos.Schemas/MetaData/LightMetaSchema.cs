using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.MetaData;

/// <summary>
///     Represents the serializable schema of the details of a light as part of the light meta data
/// </summary>
public class LightMetaSchema
{
    /// <summary>
    ///     The value of the alpha channel for the color of the light. Acceptable values are 0-32, where 32 is fully
    ///     transparent
    /// </summary>
    public byte Alpha { get; set; }

    /// <summary>
    ///     The value of the blue channel for the color of the light
    /// </summary>
    public byte Blue { get; set; }

    /// <summary>
    ///     The hour that the light ends (this would be used to update the ingame clock, but there isn't one anymore)
    /// </summary>
    public byte EndHour { get; set; }

    /// <summary>
    ///     The enum value of this light. <see cref="LightLevel" />
    /// </summary>
    public byte EnumValue { get; set; }

    /// <summary>
    ///     The value of the green channel for the color of the light
    /// </summary>
    public byte Green { get; set; }

    /// <summary>
    ///     The name of this type of light. This will be referenced in the MapInstance json.
    /// </summary>
    [JsonRequired]
    public string LightTypeName { get; set; } = null!;

    /// <summary>
    ///     The value of the red channel for the color of the light
    /// </summary>
    public byte Red { get; set; }

    /// <summary>
    ///     The hour that the light starts (this would be used to update the ingame clock, but there isn't one anymore)
    /// </summary>
    public byte StartHour { get; set; }
}