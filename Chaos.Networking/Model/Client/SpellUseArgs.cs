using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record SpellUseArgs(byte SourceSlot, byte[] ArgsData) : IReceiveArgs;