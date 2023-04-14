using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Turn" /> packet
/// </summary>
/// <param name="Direction">The direction the client is trying to turn</param>
public sealed record TurnArgs(Direction Direction) : IReceiveArgs;