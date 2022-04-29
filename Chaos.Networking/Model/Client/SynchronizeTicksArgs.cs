using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record SynchronizeTicksArgs(uint ServerTicks, uint ClientTicks) : IReceiveArgs;