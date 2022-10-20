using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record SpellUseArgs(byte SourceSlot, byte[] ArgsData) : IReceiveArgs;