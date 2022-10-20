using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record SwapSlotArgs(PanelType PanelType, byte Slot1, byte Slot2) : IReceiveArgs;