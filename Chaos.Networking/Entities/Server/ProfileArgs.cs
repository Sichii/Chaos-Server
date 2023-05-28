using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Profile" />
///     packet
/// </summary>
public sealed record ProfileArgs : ISendArgs
{
    /// <summary>
    ///     The character's secondary class
    /// </summary>
    public AdvClass AdvClass { get; set; }
    /// <summary>
    ///     The character's primary class
    /// </summary>
    public BaseClass BaseClass { get; set; }
    /// <summary>
    ///     The character's equipment, by slot
    /// </summary>
    public IDictionary<EquipmentSlot, ItemInfo?> Equipment { get; set; } = new Dictionary<EquipmentSlot, ItemInfo?>();
    /// <summary>
    ///     Whether or not the character is accepting group invites
    /// </summary>
    public bool GroupOpen { get; set; }
    /// <summary>
    ///     The character's guild name
    /// </summary>
    public string? GuildName { get; set; }
    /// <summary>
    ///     The character's guild title
    /// </summary>
    public string? GuildRank { get; set; }
    /// <summary>
    ///     The character's id
    /// </summary>
    public uint Id { get; set; }
    /// <summary>
    ///     The character's legend marks
    /// </summary>
    public ICollection<LegendMarkInfo> LegendMarks { get; set; } = new List<LegendMarkInfo>();
    /// <summary>
    ///     The character's name
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    ///     The character's nation
    /// </summary>
    public Nation Nation { get; set; }
    /// <summary>
    ///     The raw data of the character's portrait
    /// </summary>
    public byte[] Portrait { get; set; } = Array.Empty<byte>();
    /// <summary>
    ///     The character's profile text
    /// </summary>
    public string? ProfileText { get; set; }
    /// <summary>
    ///     The character's social status
    /// </summary>
    public SocialStatus SocialStatus { get; set; }
    /// <summary>
    ///     The character's currently displayed title
    /// </summary>
    public string? Title { get; set; }
}