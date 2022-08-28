using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record SwapSlotArgs(PanelType PanelType, byte Slot1, byte Slot2) : IReceiveArgs;