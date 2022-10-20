using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record SkillUseArgs(byte SourceSlot) : IReceiveArgs;