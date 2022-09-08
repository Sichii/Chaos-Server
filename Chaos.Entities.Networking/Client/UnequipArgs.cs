using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record UnequipArgs(EquipmentSlot EquipmentSlot) : IReceiveArgs;