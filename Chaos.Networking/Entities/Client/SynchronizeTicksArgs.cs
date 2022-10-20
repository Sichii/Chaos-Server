using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record SynchronizeTicksArgs(uint ServerTicks, uint ClientTicks) : IReceiveArgs;