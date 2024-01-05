using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.LightLevel" />
///     packet
/// </summary>
public sealed record LightLevelArgs : IPacketSerializable
{
    /// <summary>
    ///     The light level to be used for the current map. This is basically like time of day.
    /// </summary>
    public LightLevel LightLevel { get; set; }
}