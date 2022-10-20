using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ItemUseArgs(byte SourceSlot) : IReceiveArgs;