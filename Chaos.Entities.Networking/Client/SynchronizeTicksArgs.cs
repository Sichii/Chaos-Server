using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record SynchronizeTicksArgs(uint ServerTicks, uint ClientTicks) : IReceiveArgs;