using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record SkillUseArgs(byte SourceSlot) : IReceiveArgs;