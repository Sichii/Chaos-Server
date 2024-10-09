using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.SwapSlot" /> packet
/// </summary>
/// <remarks>
///     This packet is also used merely for moving objects, not just swapping
/// </remarks>
public sealed record SwapSlotArgs : IPacketSerializable
{
    /// <summary>
    ///     The panel of which objects are being swapped on
    /// </summary>
    public required PanelType PanelType { get; set; }

    /// <summary>
    ///     The source slot
    /// </summary>
    public required byte Slot1 { get; set; }

    /// <summary>
    ///     The destination slot
    /// </summary>
    public required byte Slot2 { get; set; }
}