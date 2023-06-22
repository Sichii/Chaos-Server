using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.SynchronizeTicks" /> packet
/// </summary>
/// <param name="ServerTicks">The ticks the client thinks the server is using</param>
/// <param name="ClientTicks">The ticks the client is using</param>
public sealed record SynchronizeTicksArgs(uint ServerTicks, uint ClientTicks) : IReceiveArgs;