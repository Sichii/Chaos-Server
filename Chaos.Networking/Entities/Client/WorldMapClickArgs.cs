using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.WorldMapClick" /> packet <br />
/// </summary>
/// <param name="UniqueId">The checksum or unique id of the node the player has clicked on</param>
/// <param name="MapId">The id of the map the node leads to</param>
/// <param name="Point">The point on the map the node leads to</param>
public sealed record WorldMapClickArgs(
    ushort UniqueId,
    ushort MapId,
    IPoint Point
) : IReceiveArgs;