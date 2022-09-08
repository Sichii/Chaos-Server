using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record SwapSlotArgs(PanelType PanelType, byte Slot1, byte Slot2) : IReceiveArgs;