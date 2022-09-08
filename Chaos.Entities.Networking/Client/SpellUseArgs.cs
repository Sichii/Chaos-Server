using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record SpellUseArgs(byte SourceSlot, byte[] ArgsData) : IReceiveArgs;