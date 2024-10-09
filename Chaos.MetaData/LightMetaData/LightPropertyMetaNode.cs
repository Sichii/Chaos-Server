using System.Drawing;
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.LightMetaData;

/// <summary>
///     A node that stores metadata about the properties of light. There can be different types of lights, such as
///     "Default", "BloodMoon", etc... whatever you want to name them
/// </summary>
public class LightPropertyMetaNode(string lightTypeName) : IMetaNode
{
    /// <summary>
    ///     The color of the light
    /// </summary>
    public Color Color { get; init; }

    /// <summary>
    ///     The hour that the light ends (this would be used to update the ingame clock, but there isn't one anymore)
    /// </summary>
    public byte EndHour { get; init; }

    /// <summary>
    ///     The enum value of this light. <see cref="LightLevel" />
    /// </summary>
    public byte EnumValue { get; init; }

    /// <summary>
    ///     The name of this type of light
    /// </summary>
    public string Name { get; init; } = lightTypeName;

    /// <summary>
    ///     The hour that the light starts (this would be used to update the ingame clock, but there isn't one anymore)
    /// </summary>
    public byte StartHour { get; init; }

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8($"{Name}_{EnumValue:X}"); //(Name_0  - Name_B) 12 total values... in normal DA it's "Default_0" - "Default_B"
        writer.WriteInt16(6);

        writer.WriteString16($"{StartHour}");
        writer.WriteString16($"{EndHour}");
        writer.WriteString16($"{Color.A}");
        writer.WriteString16($"{Color.R}");
        writer.WriteString16($"{Color.G}");
        writer.WriteString16($"{Color.B}");
    }
}