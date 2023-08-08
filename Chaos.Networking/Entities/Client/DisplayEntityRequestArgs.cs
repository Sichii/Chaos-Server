using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="ClientOpCode.DisplayEntityRequest" /> packet
///     <br />
///     I suggest not responding to this packet
/// </summary>
/// <param name="TargetId">The id of the object the client is requesting the server to display</param>
public sealed record DisplayEntityRequestArgs(uint TargetId) : IReceiveArgs;