using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.SwapSlot" /> packet
/// </summary>
/// <param name="PanelType">The panel of which objects are being swapped on</param>
/// <param name="Slot1">The source slot</param>
/// <param name="Slot2">The destination slot</param>
/// <remarks>This packet is also use merely for moving objects, not just swapping</remarks>
public sealed record SwapSlotArgs(PanelType PanelType, byte Slot1, byte Slot2) : IReceiveArgs;