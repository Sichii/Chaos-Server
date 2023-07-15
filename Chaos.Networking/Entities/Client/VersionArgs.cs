using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Version" />
///     packet
/// </summary>
/// <param name="Version">The client version as a single number</param>
public sealed record VersionArgs(ushort Version) : IReceiveArgs;