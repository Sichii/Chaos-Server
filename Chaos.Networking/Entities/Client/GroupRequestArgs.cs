using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.GroupRequest" /> packet <br />
/// </summary>
/// <param name="GroupRequestType">The type of the request</param>
/// <param name="TargetName">The name of the player the client is trying to send a group invite to</param>
public sealed record GroupRequestArgs(GroupRequestType GroupRequestType, string TargetName) : IReceiveArgs;