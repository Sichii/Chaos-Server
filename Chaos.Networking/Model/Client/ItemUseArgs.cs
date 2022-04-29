using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ItemUseArgs(byte SourceSlot) : IReceiveArgs;