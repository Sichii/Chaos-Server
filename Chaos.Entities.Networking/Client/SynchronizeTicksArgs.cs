using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record SynchronizeTicksArgs(uint ServerTicks, uint ClientTicks) : IReceiveArgs;