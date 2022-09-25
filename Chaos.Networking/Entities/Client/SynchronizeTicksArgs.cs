using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record SynchronizeTicksArgs(uint ServerTicks, uint ClientTicks) : IReceiveArgs;