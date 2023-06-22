using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.DisplayObjectRequest" /> packet
///     <br />
///     I suggest not responding to this packet
/// </summary>
/// <param name="TargetId">The id of the object the client is requesting the server to display</param>
public sealed record DisplayObjectRequestArgs(uint TargetId) : IReceiveArgs;