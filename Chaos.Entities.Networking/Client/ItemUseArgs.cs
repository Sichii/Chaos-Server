using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record ItemUseArgs(byte SourceSlot) : IReceiveArgs;