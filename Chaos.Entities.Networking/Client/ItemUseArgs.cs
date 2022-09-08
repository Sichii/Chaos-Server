using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ItemUseArgs(byte SourceSlot) : IReceiveArgs;