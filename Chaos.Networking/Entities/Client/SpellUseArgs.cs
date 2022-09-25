using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record SpellUseArgs(byte SourceSlot, byte[] ArgsData) : IReceiveArgs;