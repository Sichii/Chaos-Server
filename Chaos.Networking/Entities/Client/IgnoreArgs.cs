using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Ignore" />
///     packet
/// </summary>
/// <param name="IgnoreType">The action the client is performing in the ignore window</param>
/// <param name="TargetName">The name of the player the action is targeted at</param>
public sealed record IgnoreArgs(IgnoreType IgnoreType, string? TargetName) : IReceiveArgs;