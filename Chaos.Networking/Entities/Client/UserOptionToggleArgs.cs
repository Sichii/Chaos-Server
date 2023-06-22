using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.UserOptionToggle" /> packet
/// </summary>
/// <param name="UserOption">The option the client is trying to toggle</param>
public sealed record UserOptionToggleArgs(UserOption UserOption) : IReceiveArgs;