using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Effect" />
///     packet
/// </summary>
public sealed record EffectArgs : IPacketSerializable
{
    /// <summary>
    ///     The color of the effect
    /// </summary>
    public EffectColor EffectColor { get; set; }

    /// <summary>
    ///     The icon of the effect. This acts like a key, you can only have 1 effect with the same icon
    /// </summary>
    public byte EffectIcon { get; set; }
}