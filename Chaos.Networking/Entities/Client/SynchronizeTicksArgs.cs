using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "We don't respond to this packet atm")]
public sealed record SynchronizeTicksArgs(uint ServerTicks, uint ClientTicks) : IReceiveArgs;