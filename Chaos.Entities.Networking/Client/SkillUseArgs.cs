using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record SkillUseArgs(byte SourceSlot) : IReceiveArgs;