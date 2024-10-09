using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a legend mark in the <see cref="ServerOpCode.SelfProfile" /> and
///     <see cref="ServerOpCode.OtherProfile" /> packets
/// </summary>
public sealed record LegendMarkInfo
{
    /// <summary>
    ///     The color of the mark
    /// </summary>
    public MarkColor Color { get; set; }

    /// <summary>
    ///     The icon of the mark
    /// </summary>
    public MarkIcon Icon { get; set; }

    /// <summary>
    ///     The key of the mark. This is used as a key, as well as to "complete" quests in the player's metadata. (The event id
    ///     in the event metadata)
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    ///     The text of the mark.
    /// </summary>
    public string Text { get; set; } = null!;
}