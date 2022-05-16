using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record SwapSlotArgs(PanelType PanelType, byte Slot1, byte Slot2) : IReceiveArgs;