using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record ItemUseArgs(byte SourceSlot) : IReceiveArgs;