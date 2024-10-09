using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.CreateCharFinalize" /> packet
/// </summary>
public sealed record CreateCharFinalizeArgs : IPacketSerializable
{
    /// <summary>
    ///     The gender of the new character
    /// </summary>
    public required Gender Gender { get; set; }

    /// <summary>
    ///     The color of the new character's hair
    /// </summary>
    public required DisplayColor HairColor { get; set; }

    /// <summary>
    ///     The hairstyle to use for the new character
    /// </summary>
    public required byte HairStyle { get; set; }
}