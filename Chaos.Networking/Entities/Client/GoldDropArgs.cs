using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.GoldDrop" /> packet <br />
/// </summary>
/// <param name="Amount">The amount of gold the client is trying to drop</param>
/// <param name="DestinationPoint">The point the client is trying to drop the gold on</param>
public sealed record GoldDropArgs(int Amount, IPoint DestinationPoint) : IReceiveArgs;