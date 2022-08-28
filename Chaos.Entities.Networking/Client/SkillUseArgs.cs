using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record SkillUseArgs(byte SourceSlot) : IReceiveArgs;