using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.SelfProfile" />
///     packet
/// </summary>
public sealed record SelfProfileArgs : ISendArgs
{
    /// <summary>
    ///     The character's secondary class
    /// </summary>
    public AdvClass? AdvClass { get; set; }
    /// <summary>
    ///     The character's primary class
    /// </summary>
    public BaseClass BaseClass { get; set; }
    /// <summary>
    ///     The character's equipment, by slot
    /// </summary>
    public IDictionary<EquipmentSlot, ItemInfo> Equipment { get; set; } = new Dictionary<EquipmentSlot, ItemInfo>();
    /// <summary>
    ///     Whether or not the character is accepting group invites
    /// </summary>
    public bool GroupOpen { get; set; }
    /// <summary>
    ///     A string representing the character's group. Contains all group members in a specific format.
    /// </summary>
    /// <remarks>
    ///     The format of this string is as follows: <br />
    ///     <para>
    ///         - Starts with "Group members" <br />
    ///         - For each group member: <br />
    ///         --- If leader: "\n* {LeaderName}" <br />
    ///         --- Otherwise: "\n  {MemberName}" <br />
    ///         - Ends with "Total {Count}" <br />
    ///     </para>
    /// </remarks>
    /// <example>
    ///     "Group members\n* Sichi\n  Makiz\n  Mink\n  Bink\nTotal 4"
    /// </example>
    public string? GroupString { get; set; }
    /// <summary>
    ///     The character's guild name
    /// </summary>
    public string? GuildName { get; set; }
    /// <summary>
    ///     The character's guild title
    /// </summary>
    public string? GuildRank { get; set; }
    /// <summary>
    ///     Whether or not the character is a Master
    /// </summary>
    public bool IsMaster { get; set; }
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
    public string ProfileText { get; set; } = null!;
    /// <summary>
    ///     The character's social status
    /// </summary>
    public SocialStatus SocialStatus { get; set; }
    /// <summary>
    ///     The character's spouse name
    /// </summary>
    public string? SpouseName { get; set; }
    /// <summary>
    ///     The character's title
    /// </summary>
    public string? Title { get; set; }
}